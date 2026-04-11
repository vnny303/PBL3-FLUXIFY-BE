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
