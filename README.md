-- Tạo Database
CREATE DATABASE ShopifyCloneDB;
GO

USE ShopifyCloneDB;
GO

-- 1. Platform Users (Người dùng hệ thống)
CREATE TABLE platform_users (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    fullname NVARCHAR(255) NOT NULL,
    email NVARCHAR(255) UNIQUE NOT NULL,
    password_hash NVARCHAR(255) NOT NULL,
    phone NVARCHAR(20),
    role NVARCHAR(20) DEFAULT 'merchant', -- 'merchant' hoặc 'customer'
    is_active BIT DEFAULT 1,
    created_at DATETIME DEFAULT GETDATE()
);

-- 2. Tenants (Cửa hàng của merchant)
CREATE TABLE tenants (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    owner_id UNIQUEIDENTIFIER NOT NULL,
    subdomain NVARCHAR(100) UNIQUE NOT NULL, -- vd: shop1
    store_name NVARCHAR(255) NOT NULL,
    is_active BIT DEFAULT 1,
    FOREIGN KEY (owner_id) REFERENCES platform_users(id)
);

-- 3. Customers (Khách hàng của từng cửa hàng)
CREATE TABLE customers (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    email NVARCHAR(255) NOT NULL,
    password_hash NVARCHAR(255) NOT NULL,
    is_active BIT DEFAULT 1,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    UNIQUE(tenant_id, email) -- Email unique trong mỗi store
);

-- 4. Categories
CREATE TABLE categories (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    is_active BIT DEFAULT 1,
    FOREIGN KEY (tenant_id) REFERENCES tenants(id)
);

-- 5. Products
CREATE TABLE products (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    category_id UNIQUEIDENTIFIER,
    name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    sku NVARCHAR(100),
    price DECIMAL(18,2) NOT NULL,
    stock INT DEFAULT 0,
    is_active BIT DEFAULT 1,
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (category_id) REFERENCES categories(id)
);

-- 6. Product Attributes (màu sắc, size, etc)
CREATE TABLE product_attributes (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    product_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(255) NOT NULL, -- 'Color', 'Size'
    value NVARCHAR(255) NOT NULL, -- 'Red', 'XL'
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- 7. Orders
CREATE TABLE orders (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    customer_id UNIQUEIDENTIFIER,
    address NVARCHAR(MAX),
    status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Processing, Completed, Cancelled
    payment_method NVARCHAR(50) DEFAULT 'COD',
    payment_status NVARCHAR(50) DEFAULT 'Pending',
    total_amount DECIMAL(18,2) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (customer_id) REFERENCES customers(id)
);

-- 8. Order Items
CREATE TABLE order_items (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    order_id UNIQUEIDENTIFIER NOT NULL,
    product_id UNIQUEIDENTIFIER NOT NULL,
    selected_options NVARCHAR(MAX), -- JSON: {"Color": "Red", "Size": "XL"}
    quantity INT NOT NULL,
    unit_price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- 9. Cart Items (Giỏ hàng tạm)
CREATE TABLE cart_items (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    customer_id UNIQUEIDENTIFIER NOT NULL,
    product_id UNIQUEIDENTIFIER NOT NULL,
    selected_options NVARCHAR(MAX),
    quantity INT NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES customers(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- Thêm dữ liệu mẫu
-- Merchant
INSERT INTO platform_users (id, fullname, email, password_hash, role) 
VALUES (NEWID(), 'John Merchant', 'merchant@test.com', 'hashed_password', 'merchant');

-- Tenant
DECLARE @merchantId UNIQUEIDENTIFIER = (SELECT id FROM platform_users WHERE email = 'merchant@test.com');
INSERT INTO tenants (id, owner_id, subdomain, store_name) 
VALUES (NEWID(), @merchantId, 'shop1', 'My Awesome Store');

GO
