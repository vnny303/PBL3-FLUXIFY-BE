Luc_21h/6/3/2026
\*Model

- cart
  Tạo mới — entity Cart, ánh xạ bảng carts, quan hệ 1:1 với Customer, 1:N với CartItem
- cartitem
  Đổi ProductId → ProductSkuId, đổi CustomerId → CartId, navigation Product → ProductSku, xóa SelectedOptions
- orderitem
  Đổi ProductId → ProductSkuId, navigation Product → ProductSku
- ProductSku
  Thêm collections CartItems và OrderItems
- Product
  Xóa CartItems và OrderItems (chuyển về ProductSku)
- Customer
  Đổi navigation CartItems → Cart? (1:1 với Cart)
  \*Data
- Data/AppDbContext.cs
  Thêm DbSet<Cart>, cấu hình quan hệ Cart-Customer (1:1), CartItem-Cart (N:1), CartItem-ProductSku (N:1), OrderItem-ProductSku (N:1); sửa 3 lỗi logic cũ
  \*Controllers
- CartItemsController.cs
  Viết lại hoàn toàn — dùng productSkuId, tự tạo Cart nếu chưa có, bỏ selectedOptions
- OrdersController.cs
  Cập nhật include: ThenInclude(oi => oi.ProductSku).ThenInclude(s => s.Product)
- AdminController.cs
  Tạo mới — GET /api/admin/platformUsers, DELETE /api/admin/platformUsers/{id}
  \*Migrations & Database
  Migration
- 20260306132545_UpdateCartAndOrderItems
  Đổi tên cột product_id→product_sku_id, customer_id→cart_id trong cart_items; đổi product_id→product_sku_id trong order_items; tạo bảng carts
- 20260306135908_RemoveCartItemSelectedOptions
  Xóa cột selected_options khỏi bảng cart_items

================================================================================
Hoa_2h/07/03/2026
\*DTO: folder này chứa các class giúp ẩn thông tin nhạy cảm và tránh vòng lặp circular reference khi serialize (sẽ còn được cập nhật để áp dụng thêm các Data Validation)

- Ví dụ:
  - Ẩn thông tin nhạy cảm: PlatformUser và Customer -> DTO sẽ không trả về hash mật khẩu
  - Tránh circular reference & over-fetching:
    -- PlatformUser: Tenants → Owner → Tenants...
    -- Customer: Cart → Customer → Cart... ;
    Orders, Tenant
    -- Tenant: Customers, Products, Orders, Owner
    -- Order: Customer → Orders... ;
    OrderItems, Tenant
    -- Cart: Customer → Cart... ; CartItems
    -- Product: ProductSkus, Tenant, Category
    -- ProductSku: Product → ProductSkus... ;
    CartItems, OrderItems

\*Mapper: chuyển từ dữ liệu trong database sang DTO (giúp bảo mật), được sử dụng trong các Controllers (hiện tại chưa sử dụng)

\*AppDbContext: refactor lại để gọn hơn (chuyển từ Database-first scaffold sang Code-first clean design)

================================================================================
Hoa_02/04/2026
\*Models: thêm Cart.cs, quan hệ Cart - 1,1 - Customer, Cart - n,1 - Tenant, Cart - 1,n - CartItem

\*Interfaces: chứa các interface CRUD của Repository. Khi thêm/sửa CRUD thì thêm vào đây (lưu ý chỉnh sửa tương ứng bên Repository)

\*Repository: chứa các CRUD được sử dụng trong Controllers (để các controller nhìn gọn hơn)

\*DTOs:

- Các file Create...RequestDto.cs, Update...RequestDto.cs: model được sử dụng trong các hàm Create, Update trong controller thay vì sử dụng thẳng các models trong folder Models

\*Mapper: chuyển từ Model sang DTO và CreateDTO sang Model (sử dụng trong api POST)

\*Controllers:

- TenantController.cs: hiện tại chỉ đang có api bên phần view của merchant, view của customer cập nhật sau (1 class mới)
- CartController.cs: chỉ có GET cart, các hàm crud trong giỏ hàng liên quan bên CartItem

\*Database.sql: thêm dữ liệu mới

================================================================================

Luc_22h/3/4/2026

1. Thêm các file Create/Update ở DTO
2. Bổ sung hàm chuẩn hóa Mapper theo 2 chiều
3. Refactor controoler từ JsonElement/string sang DTO + mapper

================================================================================

Hoa_23h/10/04/2026

1. Thêm property List<string> imgUrls vào Model, DTO và Mapper của Product, thêm string imgUrl vào Model, DTO và Mapper của ProductSku
2. chỉnh endpoint /customer/login và customer/register nhận subdomain từ query (không phải nhận từ body). chi tiết đọc kỹ trong API SPEC

================================================================================

Hoa_11/04/2026
Thêm folder Helpers: chứa các class hỗ trợ cho việc filter, sort, pagination,...
================================================================================

Luc_21h/11/4/2026

### Added

- Bổ sung hệ thống Query Object dùng chung để chuẩn hóa filter/sort/paging:
  - `QueryBase`
  - `QueryTenant`, `QueryTenantOwner`
  - `QueryPlatformUser`, `QueryCustomer`, `QueryCategory`
  - `QueryProduct`, `QueryProductSku`
  - `QueryCart`, `QueryCartItem`
  - `QueryOrder`, `QueryOrderItem`
- Tạo Service layer cho scope Auth + Tenant:
  - `Services/Interfaces/IAuthService`, `Services/Interfaces/ITenantService`
  - `Services/Implementations/AuthService`, `Services/Implementations/TenantService`
  - `Services/Common/ServiceResult<T>` để chuẩn hóa kết quả trả về từ service.

### Changed

- Refactor `AuthController` để gọi `IAuthService` thay vì xử lý trực tiếp toàn bộ nghiệp vụ trong controller.
- Refactor `TenantsController` để gọi `ITenantService` thay vì orchestration trực tiếp qua repository.
- Cập nhật `Program.cs`:
  - đăng ký DI cho service bằng `AddScoped` (`IAuthService`, `ITenantService`).
- Cập nhật truy vấn tenant theo owner:
  - áp dụng filter theo query object,
  - thêm paging (`Skip/Take`),
  - thêm sort theo nhiều field,
  - thêm fallback sort ổn định theo `Id` khi `sortBy` không hợp lệ,
  - thêm `AsNoTracking` cho query chỉ đọc.

### Fixed

- Resolve toàn bộ merge conflict còn sót trong `QueryTenant` và `TenantRepository`.
- Vá rủi ro phân quyền ở `CustomersController`:
  - bổ sung check owner cho `GetCustomerByCart` và `CreateCustomer`.
- Đồng bộ behavior claim merchant trong auth flow (token merchant không phụ thuộc `tenantId` claim).

### Notes

- Build solution thành công sau các cập nhật trên.
- Còn warning kỹ thuật cũ (migrations/obsolete API/nullability) chưa xử lý trong đợt này.


Luc_16h/16/4/2026
Refactor lại luồng Order cho merchant

Chuẩn hóa xác thực theo userId claim + role merchant ở OrdersController.cs:10.

Bỏ các endpoint placeholder chưa triển khai trong OrdersController.cs.

Đồng bộ chữ ký service/order giữa interface và implementation ở IOrderService.cs:9 và OrderService.cs:24.

Thêm kiểm tra quyền tenant owner trong service Order ở OrderService.cs:26.

Fix luồng CartItem hiển thị tên SKU/sản phẩm

Mở rộng DTO cart item để trả dữ liệu hiển thị (ProductName, SkuDisplayName, UnitPrice, ảnh SKU...) ở CartItemDto.cs:13.

Map dữ liệu hiển thị trong CartMapper.cs:34.

Include thêm Product từ ProductSku trong CartItemRepository.cs:22.

Sau khi AddToCart, re-query lại item để response có đủ thông tin hiển thị ở CartItemService.cs:57.

Triển khai mới luồng Customer Order

Thêm controller mới cho customer order tại CustomerOrdersController.cs:11.

Endpoint mới:

GET /api/customer/orders

GET /api/customer/orders/{orderId}

POST /api/customer/orders/checkout

PUT /api/customer/orders/{orderId}/cancel

Thêm DTO checkout ở CheckoutOrderRequestDto.cs:5.

Mở rộng service contract customer flow ở IOrderService.cs:15.

Implement logic customer flow trong OrderService.cs:191.

Sửa lỗi runtime phát sinh khi test

Sửa lỗi Collection was modified trong checkout bằng snapshot cart items ở OrderService.cs:299.

Đồng bộ database

Đã chạy migration để thêm cột ảnh product/sku (img_urls, img_url), tương ứng migration 20260414060029_addImgUrlToProductAndProductSku.cs.

Kết quả test

Đã test E2E thành công các bước customer order: add cart item -> checkout -> get my orders -> get order detail -> cancel order -> verify status Cancelled.

Build hiện pass, còn warning cũ (nullability/obsolete API) chưa xử lý trong đợt này.

================================================================================

Luc_17/04/2026

Theme customize + Tenant API refactor

### Added

1. Hỗ trợ lưu cấu hình giao diện storefront trực tiếp trên Tenant:
  - Thêm 2 cột mới trong bảng `tenants`:
    - `content_config`
    - `theme_config`
  - Migration: `20260417111822_AddTenantThemeAndContentConfig`

2. Bổ sung schema DTO cho cấu hình storefront:
  - `StorefrontContentConfigDto` (home/about)
  - `StorefrontThemeConfigDto` (colors/typography/layout/components)
  - `StorefrontTenantLookupDto` cho endpoint public lookup theo subdomain

3. Chuẩn hóa API cập nhật theo resource cho theme/content:
  - `PATCH /api/tenants/subdomain/{subdomain}/content`
  - `PATCH /api/tenants/subdomain/{subdomain}/theme`

### Changed

1. Refactor luồng map Tenant config JSON:
  - Serialize/deserialize qua mapper để chuyển đổi 2 chiều giữa JSON DB <-> DTO strongly typed.
  - `CreateTenant` và `RegisterMerchant` khởi tạo cấu hình content/theme mặc định.

2. PUT tenant (`PUT /api/tenants/{id}`) chỉ còn xử lý thông tin tenant lõi:
  - `subdomain`, `storeName`, `isActive`
  - Không còn cập nhật trực tiếp content/theme trong PUT.

### Fixed

1. Sửa các check quyền Tenant bị đảo logic trong service:
  - Chuẩn hóa thứ tự xử lý: `404 not found` -> `403 forbidden`.

2. Sửa lỗi tiềm ẩn runtime ở repository:
  - Bỏ `Include` anonymous expression không hợp lệ.
  - Đổi sang include navigation hợp lệ và tối ưu query lookup subdomain.

3. Hardening contract để tránh silent break ở client cũ:
  - Nếu client vẫn gửi `contentConfig`/`themeConfig` vào `PUT /api/tenants/{id}` -> trả `400` kèm hướng dẫn dùng PATCH resource mới.

4. Lookup storefront trả `404` khi tenant không active để tránh lộ trạng thái store.
