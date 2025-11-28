# Hướng dẫn cấu hình Email Settings

## Cấu hình EmailSettings trong appsettings.json

### 1. Sử dụng Gmail (Khuyến nghị)

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": "587",
  "SmtpUsername": "your-email@gmail.com",
  "SmtpPassword": "your-app-password",
  "FromEmail": "apmms.manage.services@gmail.com",
  "FromName": "APMMS System"
}
```

#### Các bước cấu hình Gmail:

1. **SmtpServer**: Giữ nguyên `"smtp.gmail.com"`

2. **SmtpPort**: 
   - `587` - Sử dụng TLS (khuyến nghị)
   - `465` - Sử dụng SSL (nếu 587 không hoạt động)

3. **SmtpUsername**: 
   - Điền email Gmail của bạn (ví dụ: `apmms.manage.services@gmail.com`)

4. **SmtpPassword**: 
   - ⚠️ **QUAN TRỌNG**: Không dùng mật khẩu Gmail thông thường!
   - Phải tạo **App Password** (Mật khẩu ứng dụng):
     - Vào: https://myaccount.google.com/apppasswords
     - Hoặc: Google Account → Security → 2-Step Verification → App passwords
     - Chọn "Mail" và "Other (Custom name)" → Đặt tên "APMMS"
     - Copy mật khẩu 16 ký tự (không có dấu cách)
     - Dán vào `SmtpPassword`

5. **FromEmail**: 
   - Email hiển thị trong phần "From" của email gửi đi
   - ⚠️ **Thường giống `SmtpUsername`** (cùng một email)
   - Ví dụ: `apmms.manage.services@gmail.com`
   - **Lưu ý**: Với Gmail, `FromEmail` phải giống `SmtpUsername`, không thể gửi từ email khác

6. **FromName**: 
   - Tên hiển thị trong email
   - Ví dụ: `"APMMS System"` hoặc `"Hệ thống APMMS"`

### 2. Sử dụng Outlook/Hotmail

```json
"EmailSettings": {
  "SmtpServer": "smtp-mail.outlook.com",
  "SmtpPort": "587",
  "SmtpUsername": "your-email@outlook.com",
  "SmtpPassword": "your-password",
  "FromEmail": "your-email@outlook.com",
  "FromName": "APMMS System"
}
```

### 3. Sử dụng Email Server riêng

```json
"EmailSettings": {
  "SmtpServer": "mail.yourdomain.com",
  "SmtpPort": "587",
  "SmtpUsername": "noreply@yourdomain.com",
  "SmtpPassword": "your-password",
  "FromEmail": "noreply@yourdomain.com",
  "FromName": "APMMS System"
}
```

### 4. Ví dụ cấu hình thực tế

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": "587",
  "SmtpUsername": "apmms.manage.services@gmail.com",
  "SmtpPassword": "abcd efgh ijkl mnop",
  "FromEmail": "apmms.manage.services@gmail.com",
  "FromName": "Hệ thống APMMS"
}
```

**Lưu ý**: 
- App Password của Gmail có 16 ký tự, có thể có dấu cách (bạn có thể bỏ dấu cách hoặc giữ nguyên)
- Nếu dùng mật khẩu thường, Gmail sẽ từ chối kết nối

### 5. Kiểm tra cấu hình

Sau khi cấu hình, khởi động lại ứng dụng và thử chức năng "Quên mật khẩu". Nếu có lỗi, kiểm tra:
- Console log để xem thông báo lỗi
- Đảm bảo đã bật 2-Step Verification trên Gmail
- Kiểm tra App Password đã được tạo đúng

### 6. Bảo mật

⚠️ **KHÔNG commit file appsettings.json có chứa mật khẩu thật vào Git!**

- Sử dụng `appsettings.Development.json` cho môi trường dev
- Sử dụng User Secrets hoặc Environment Variables cho production
- Thêm `appsettings.json` vào `.gitignore` nếu chứa thông tin nhạy cảm

