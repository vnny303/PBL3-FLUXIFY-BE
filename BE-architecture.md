```
├── 📁 Controllers
│   ├── 📄 AdminController.cs
│   ├── 📄 AnalyticsController.cs
│   ├── 📄 AuthController.cs
│   ├── 📄 CartController.cs
│   ├── 📄 CategoriesController.cs
│   ├── 📄 CustomerAddressesController.cs
│   ├── 📄 CustomerOrdersController.cs
│   ├── 📄 CustomerReviewsController.cs
│   ├── 📄 CustomerSelfAddressesController.cs
│   ├── 📄 CustomersController.cs
│   ├── 📄 OrdersController.cs
│   ├── 📄 ProductReviewsController.cs
│   ├── 📄 ProductsController.cs
│   └── 📄 TenantsController.cs
├── 📁 DTOs
│   ├── 📁 Analytics
│   │   ├── 📄 TenantAnalyticsBreakdownItemDto.cs
│   │   ├── 📄 TenantAnalyticsDashboardDto.cs
│   │   ├── 📄 TenantAnalyticsInventoryAlertDto.cs
│   │   ├── 📄 TenantAnalyticsOverviewDto.cs
│   │   ├── 📄 TenantAnalyticsRatingOverviewDto.cs
│   │   ├── 📄 TenantAnalyticsTimeSeriesPointDto.cs
│   │   └── 📄 TenantAnalyticsTopProductDto.cs
│   ├── 📁 Auth
│   │   ├── 📄 LoginRequest.cs
│   │   ├── 📄 RegisterCustomerRequest.cs
│   │   └── 📄 RegisterMerchantRequest.cs
│   ├── 📁 Cart
│   │   ├── 📄 CartDto.cs
│   │   ├── 📄 CartItemDto.cs
│   │   ├── 📄 CreateCartItemRequestDto.cs
│   │   ├── 📄 CreateCartRequestDto.cs
│   │   └── 📄 UpdateCartItemRequestDto.cs
│   ├── 📁 Cartegory
│   │   ├── 📄 CategoryDto.cs
│   │   ├── 📄 CreateCategoryRequestDto.cs
│   │   └── 📄 UpdateCategoryRequestDto.cs
│   ├── 📁 Customer
│   │   ├── 📄 CreateCustomerRequestDto.cs
│   │   ├── 📄 CustomerDto.cs
│   │   └── 📄 UpdateCustomerRequestDto.cs
│   ├── 📁 CustomerAddress
│   │   ├── 📄 CreateCustomerAddressDto.cs
│   │   ├── 📄 CustomerAddressDto.cs
│   │   └── 📄 UpdateCustomerAddressDto.cs
│   ├── 📁 Order
│   │   ├── 📄 CheckoutOrderRequestDto.cs
│   │   ├── 📄 CreateOrderItemRequestDto.cs
│   │   ├── 📄 CreateOrderRequestDto.cs
│   │   ├── 📄 OrderDto.cs
│   │   ├── 📄 OrderItemDto.cs
│   │   └── 📄 UpdateOrderStatusRequestDto.cs
│   ├── 📁 PlatformUser
│   │   └── 📄 PlatformUserDto.cs
│   ├── 📁 Product
│   │   ├── 📄 CreateProductRequestDto.cs
│   │   ├── 📄 DetailSectionDto.cs
│   │   ├── 📄 ProductDetailDto.cs
│   │   ├── 📄 ProductDto.cs
│   │   ├── 📄 SpecificationDto.cs
│   │   └── 📄 UpdateProductRequestDto.cs
│   ├── 📁 ProductSku
│   │   ├── 📄 CreateProductSkuRequestDto.cs
│   │   ├── 📄 ProductSkuDto.cs
│   │   └── 📄 UpdateProductSkuRequestDto.cs
│   ├── 📁 Review
│   │   ├── 📄 CreateReviewRequestDto.cs
│   │   ├── 📄 ReviewDto.cs
│   │   ├── 📄 ReviewSummaryDto.cs
│   │   └── 📄 UpdateReviewRequestDto.cs
│   ├── 📁 Tenant
│   │   ├── 📄 CreateTenantRequestDto.cs
│   │   ├── 📄 StorefrontContentConfigDto.cs
│   │   ├── 📄 StorefrontTenantLookupDto.cs
│   │   ├── 📄 StorefrontThemeConfigDto.cs
│   │   ├── 📄 TenantDto.cs
│   │   └── 📄 UpdateTenantRequestDto.cs
├── 📁 Data
│   └── 📄 AppDbContext.cs
├── 📁 Helpers
│   ├── 📄 QueryBase.cs
│   ├── 📄 QueryCartItem.cs
│   ├── 📄 QueryCategory.cs
│   ├── 📄 QueryCustomer.cs
│   ├── 📄 QueryOrder.cs
│   ├── 📄 QueryOrderItem.cs
│   ├── 📄 QueryProduct.cs
│   ├── 📄 QueryProductSku.cs
│   ├── 📄 QueryReview.cs
│   ├── 📄 QueryTenant.cs
│   └── 📄 QueryTenantAnalytics.cs
├── 📁 Mapper
│   ├── 📄 CartMapper.cs
│   ├── 📄 CategoryMapper.cs
│   ├── 📄 CustomerAddressMapper.cs
│   ├── 📄 CustomerMapper.cs
│   ├── 📄 OrderMapper.cs
│   ├── 📄 PlatformUserMapper.cs
│   ├── 📄 ProductMapper.cs
│   ├── 📄 ProductSkuMapper.cs
│   ├── 📄 ReviewMapper.cs
│   ├── 📄 TenantMapper.cs
├── 📁 Migrations
├── 📁 Models
│   ├── 📄 Cart.cs
│   ├── 📄 CartItem.cs
│   ├── 📄 Category.cs
│   ├── 📄 Customer.cs
│   ├── 📄 CustomerAddress.cs
│   ├── 📄 Order.cs
│   ├── 📄 OrderItem.cs
│   ├── 📄 PlatformUser.cs
│   ├── 📄 Product.cs
│   ├── 📄 ProductSku.cs
│   ├── 📄 Review.cs
│   ├── 📄 Tenant.cs
├── 📁 Properties
├── 📁 Repository
│   ├── 📁 Implementations
│   │   ├── 📄 CartItemRepository.cs
│   │   ├── 📄 CartRepository.cs
│   │   ├── 📄 CategoryRepository.cs
│   │   ├── 📄 CustomerAddressRepository.cs
│   │   ├── 📄 CustomerRepository.cs
│   │   ├── 📄 OrderItemRepository.cs
│   │   ├── 📄 OrderRepository.cs
│   │   ├── 📄 PlatformUserRepository.cs
│   │   ├── 📄 ProductRepository.cs
│   │   ├── 📄 ProductSkuRepository.cs
│   │   ├── 📄 ReviewRepository.cs
│   │   └── 📄 TenantRepository.cs
│   └── 📁 Interfaces
│       ├── 📄 ICartItemRepository.cs
│       ├── 📄 ICartRepository.cs
│       ├── 📄 ICategoryRepository.cs
│       ├── 📄 ICustomerAddressRepository.cs
│       ├── 📄 ICustomerRepository.cs
│       ├── 📄 IOrderItemRepository.cs
│       ├── 📄 IOrderRepository.cs
│       ├── 📄 IPlatformUserRepository.cs
│       ├── 📄 IProductRepository.cs
│       ├── 📄 IProductSkuRepository.cs
│       ├── 📄 IReviewRepository.cs
│       └── 📄 ITenantRepository.cs
├── 📁 Services
│   ├── 📁 Implementations
│   │   ├── 📄 AdminService.cs
│   │   ├── 📄 AnalyticsService.cs
│   │   ├── 📄 AuthService.cs
│   │   ├── 📄 CartService.cs
│   │   ├── 📄 CategoryService.cs
│   │   ├── 📄 CustomerAddressService.cs
│   │   ├── 📄 CustomerService.cs
│   │   ├── 📄 OrderService.cs
│   │   ├── 📄 ProductService.cs
│   │   ├── 📄 ReviewService.cs
│   │   └── 📄 TenantService.cs
│   ├── 📁 Interfaces
│   │   ├── 📄 IAdminService.cs
│   │   ├── 📄 IAnalyticsService.cs
│   │   ├── 📄 IAuthService.cs
│   │   ├── 📄 ICartService.cs
│   │   ├── 📄 ICategoryService.cs
│   │   ├── 📄 ICustomerAddressService.cs
│   │   ├── 📄 ICustomerService.cs
│   │   ├── 📄 IOrderService.cs
│   │   ├── 📄 IProductService.cs
│   │   ├── 📄 IReviewService.cs
│   │   └── 📄 ITenantService.cs
│   └── 📄 ServiceResult.cs
├── ⚙️ .gitignore
├── 📝 Changelog.md
├── 📄 Database.sql
├── 📄 PBL3-FLUXIFY-BE.sln
├── 📄 Program.cs
├── 📝 README.md
├── 📄 ShopifyAPI.csproj
├── 📄 ShopifyAPI.csproj.lscache
├── 📄 ShopifyAPI.http
└── ⚙️ appsettings.example.json
```
