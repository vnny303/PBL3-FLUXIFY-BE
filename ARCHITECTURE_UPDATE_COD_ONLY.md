# Architecture Update: COD-only Checkout

Fluxify đã bỏ luồng QR / bank transfer. Những module sau đã bị loại khỏi kiến trúc runtime:

- TenantPaymentSettingsController
- TenantPaymentSetting model/DTO/mapper
- TenantPaymentSetting repository/service/interfaces
- Public `/api/tenants/{tenantId}/payment-settings` endpoint
- Bank-transfer/QR fields in checkout response

Checkout hiện chỉ nhận `paymentMethod = COD`.

> Lưu ý: các migration cũ vẫn còn để database có thể được dựng lại từ đầu theo lịch sử migration. Migration mới `20260517020000_RemoveQrPaymentAndBankTransfer` sẽ drop phần QR/bank-transfer khỏi schema hiện tại.
