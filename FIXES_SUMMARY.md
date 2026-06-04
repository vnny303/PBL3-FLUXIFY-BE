# Fluxify BE - Production Hardening Patch

Bản này tập trung sửa các lỗi production-blocker đã review trước đó. Đây không phải là rewrite hoàn toàn sang Clean Architecture nhiều project, mà là một lượt refactor an toàn để project hiện tại bớt rủi ro khi chạy thật.

## Nhóm lỗi đã sửa

### 1. Security

- `Controllers/AdminController.cs`: bật `[Authorize(Roles = "admin")]` cho toàn bộ Admin API. Trước đó comment authorization khiến API quản trị có thể bị gọi công khai.
- `Program.cs`: bỏ CORS `SetIsOriginAllowed(_ => true)`; thay bằng whitelist `Cors:AllowedOrigins`.
- `Program.cs`: kiểm tra `Jwt:Key` không được rỗng và phải đủ dài.
- `Program.cs`: thêm response chuẩn cho `401` và `403` có `traceId`.
- `appsettings.json` và `appsettings.example.json`: bỏ secret thật, thay bằng placeholder cấu hình.
- `Controllers/CustomerAddressesController.cs`: khóa endpoint quản lý địa chỉ bằng `[Authorize(Roles = "admin,merchant")]`, đưa `tenantId` vào route rõ ràng, kiểm tra tenant owner trước khi thao tác.
- Đã gỡ toàn bộ `TenantPaymentSettingsController` / `TenantPaymentSetting` vì hệ thống chuyển sang COD-only, không dùng QR/bank transfer.

### 2. Transaction consistency và inventory safety

- `Services/Implementations/OrderService.cs`: rewrite phần tạo order, checkout, hủy order.
- Checkout đã có database transaction.
- Trừ tồn kho chuyển sang atomic SQL update:
  - chỉ trừ nếu SKU thuộc đúng tenant;
  - chỉ trừ nếu `stock >= quantity`;
  - tránh race condition oversell.
- Hủy đơn hàng thực hiện trong transaction và cộng lại stock.
- Merchant create order không tin `UnitPrice` từ client; giá luôn lấy từ SKU trong DB.
- Checkout validate địa chỉ thuộc đúng customer và tenant.
- Order code không còn dùng `Count + 1`, tránh trùng khi concurrent.

### 3. Money type

- Đổi các field tiền từ `double` sang `decimal`:
  - `Models/Order.cs`
  - `Models/OrderItem.cs`
  - `Models/ProductSku.cs`
  - DTO order/product SKU/cart/analytics liên quan
  - query helper liên quan đến price/total
  - repository order item
  - analytics service
- Lý do: `double` có sai số floating point, không dùng cho tiền trong ecommerce.

### 4. Business rule / order workflow

- `UpdateOrderStatusRequestDto.cs`: thêm validation trạng thái order hợp lệ.
- `OrderService.cs`: thêm state transition rule, không cho chuyển trạng thái tùy tiện.
- Không cho xóa vật lý đơn hàng đã xử lý. Đơn đã xử lý nên lưu trữ/audit thay vì delete.
- Payment method được normalize và chỉ nhận `COD`.

### 5. Product/SKU bug

- `Services/Implementations/ProductService.cs`: sửa bug tạo SKU bị nhân đôi khi create product. Mapper đã gắn SKU vào Product, service không tạo SKU lần 2 nữa.
- Product create/update SKU được bọc transaction để tránh dữ liệu nửa vời.
- Naming `imgUrl/imgUrls` đổi sang `ImgUrl/ImgUrls` đúng convention C#, nhưng JSON output vẫn là `imgUrl/imgUrls` nhờ camelCase config.

### 6. Cart safety

- `Services/Implementations/CartService.cs`: không cho update cart item bằng cách đổi sang SKU khác. Muốn đổi SKU thì xóa item cũ rồi thêm item mới.
- `Data/AppDbContext.cs`: thêm unique index cho `(TenantId, CustomerId)` trên cart và `(CartId, ProductSkuId)` trên cart item để giảm duplicate data.

### 7. Database hardening

- `Data/AppDbContext.cs`: thêm composite indexes cho các query thường gặp:
  - order theo tenant/date;
  - order theo tenant/customer/date;
  - order theo tenant/status;
  - order theo tenant/paymentStatus;
  - product theo tenant/category;
  - cart unique theo tenant/customer;
  - cart item unique theo cart/SKU.
- Thêm check constraints:
  - amount không âm;
  - quantity > 0;
  - price/stock không âm;
  - review rating trong khoảng 1-5.
- `Migrations/20260514000000_ProductionHardening.cs`: migration thủ công cho các index/constraint mới.

### 8. Auth validation và consistency

- `LoginRequest.cs`, `RegisterCustomerRequest.cs`, `RegisterMerchantRequest.cs`: thêm validation email/password/subdomain/store name.
- `AuthService.cs`: normalize email/subdomain trước khi lookup và register.
- Register merchant/customer được bọc transaction để tránh tạo user mà tenant/cart không tạo được.

### 9. Runtime error handling

- `Middleware/ExceptionHandlingMiddleware.cs`: thêm global exception middleware để không leak stack trace và luôn trả JSON lỗi có `traceId`.

### 10. Bug cụ thể đã sửa

- `Services/Implementations/TenantService.cs`: sửa lỗi truyền nhầm `order.Id` vào tham số `tenantId` khi xóa order item. Đây là bug gây xóa tenant fail hoặc dữ liệu mồ côi.

## Những việc vẫn nên làm tiếp nếu muốn lên chuẩn enterprise thật

- Tách solution thành `Api/Application/Domain/Infrastructure/Tests`.
- Bỏ pattern repository tự `SaveChangesAsync`, thay bằng Unit of Work/Command Handler commit một lần.
- Thêm test integration cho checkout, cancel order, status transition, authorization.
- Thêm refresh token, revoke token, rate limit login.
- Thêm soft delete/audit log/outbox/background job.
- Tối ưu product list bằng projection thay vì include sâu toàn bộ review.
- Chuẩn hóa response envelope toàn bộ API.
