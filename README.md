Luc_21h/6/3/2026
*Model
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
*Data
- Data/AppDbContext.cs	
Thêm DbSet<Cart>, cấu hình quan hệ Cart-Customer (1:1), CartItem-Cart (N:1), CartItem-ProductSku (N:1), OrderItem-ProductSku (N:1); sửa 3 lỗi logic cũ
*Controllers
- CartItemsController.cs	
Viết lại hoàn toàn — dùng productSkuId, tự tạo Cart nếu chưa có, bỏ selectedOptions
- OrdersController.cs	
Cập nhật include: ThenInclude(oi => oi.ProductSku).ThenInclude(s => s.Product)
- AdminController.cs	
Tạo mới — GET /api/admin/platformUsers, DELETE /api/admin/platformUsers/{id}
*Migrations & Database
Migration
- 20260306132545_UpdateCartAndOrderItems	
Đổi tên cột product_id→product_sku_id, customer_id→cart_id trong cart_items; đổi product_id→product_sku_id trong order_items; tạo bảng carts
- 20260306135908_RemoveCartItemSelectedOptions	
Xóa cột selected_options khỏi bảng cart_items