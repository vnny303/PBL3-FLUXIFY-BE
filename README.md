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

Luc_22h/3/4/2026
1. Thêm các file Create/Update ở DTO
2. Bổ sung hàm chuẩn hóa Mapper theo 2 chiều 
3. Refactor controoler từ JsonElement/string sang DTO + mapper

