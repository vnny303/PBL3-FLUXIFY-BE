# Fluxify API (Backend)

Hướng dẫn nhanh để chạy source backend local.

## 1. Yêu cầu môi trường

- .NET SDK 10.0
- SQL Server (khuyến nghị SQL Server Express hoặc LocalDB)
- Git (nếu cần clone source)

Kiểm tra SDK đã cài:

```bash
dotnet --version
```

## 2. Cấu hình project

Từ thư mục gốc project, tạo file `appsettings.json` từ mẫu:

```bash
copy appsettings.example.json appsettings.json
```

Mở `appsettings.json` và cập nhật:

- `ConnectionStrings:DefaultConnection` theo SQL Server trên máy bạn
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` theo môi trường local

Giải thích nhanh phần JWT:

- `Jwt:Key`: chuỗi bí mật để ký token (tự tạo, không lấy từ internet).
- `Jwt:Issuer`: tên hệ thống phát hành token (tự đặt, ví dụ `FluxifyAPI.Local`).
- `Jwt:Audience`: đối tượng sử dụng token (tự đặt, ví dụ `FluxifyClient.Local`).

Lưu ý: `Issuer` và `Audience` phải giống nhau giữa lúc tạo token và lúc validate token.

## 3. Restore package

```bash
dotnet restore
```

## 4. Khởi tạo database

Project đã có sẵn migration trong thư mục `Migrations`. Chạy lệnh sau để tạo/cập nhật DB schema:

```bash
dotnet ef database update
```

Nếu máy chưa có công cụ EF CLI:

```bash
dotnet tool install --global dotnet-ef
```

Sau đó chạy lại:

```bash
dotnet ef database update
```

## 5. Chạy API

```bash
dotnet run
```

Mặc định app chạy ở:

- `http://localhost:5119`
- `https://localhost:7189`

## 6. Test API

- Trang tài liệu API (Scalar): `http://localhost:5119/scalar`
- Có thể dùng thêm file `ShopifyAPI.http` để test endpoint trong VS Code

## 7. Lỗi thường gặp

- Lỗi kết nối DB: kiểm tra lại `DefaultConnection` và đảm bảo SQL Server/LocalDB đang hoạt động.
- Lỗi JWT: kiểm tra `Jwt:Key`, `Issuer`, `Audience` đã điền giá trị hợp lệ.
- Lỗi `dotnet ef` không nhận lệnh: cài `dotnet-ef` global tool rồi mở terminal mới.

## 8. Lệnh nhanh

```bash
dotnet restore
dotnet ef database update
dotnet run
```
