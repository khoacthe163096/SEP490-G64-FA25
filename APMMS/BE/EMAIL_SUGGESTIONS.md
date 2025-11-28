# Đề xuất Email cho hệ thống APMMS

## Các email đề xuất (theo thứ tự ưu tiên)

### 1. Email chuyên nghiệp nhất (Khuyến nghị)
```
apmms.system@gmail.com
```
- ✅ Ngắn gọn, dễ nhớ
- ✅ Thể hiện đây là email hệ thống
- ✅ Chuyên nghiệp

### 2. Email với tên đầy đủ
```
apmms.management@gmail.com
apmms.support@gmail.com
apmms.noreply@gmail.com
```

### 3. Email với tên tiếng Việt (nếu muốn)
```
hethong.apmms@gmail.com
hotro.apmms@gmail.com
```

### 4. Email chi tiết hơn
```
apmms.auto.parts@gmail.com
apmms.maintenance@gmail.com
apmms.service@gmail.com
```

## Cách tạo email Gmail mới

### Bước 1: Tạo tài khoản Gmail
1. Vào: https://accounts.google.com/signup
2. Điền thông tin:
   - Họ và tên: `APMMS System` hoặc `Hệ thống APMMS`
   - Tên người dùng: Chọn một trong các email đề xuất ở trên
   - Mật khẩu: Tạo mật khẩu mạnh (ít nhất 8 ký tự)
   - Xác nhận mật khẩu
   - Số điện thoại: Điền số điện thoại của bạn (bắt buộc)
   - Email phục hồi: Điền email cá nhân của bạn (để khôi phục nếu quên mật khẩu)

### Bước 2: Xác minh tài khoản
- Google sẽ gửi mã xác minh đến số điện thoại
- Nhập mã để xác minh

### Bước 3: Bật 2-Step Verification
1. Vào: https://myaccount.google.com/security
2. Tìm "2-Step Verification" → Bật
3. Làm theo hướng dẫn

### Bước 4: Tạo App Password
1. Vào: https://myaccount.google.com/apppasswords
2. Chọn "Mail" → "Other (Custom name)"
3. Đặt tên: `APMMS System`
4. Click "Generate"
5. Copy mật khẩu 16 ký tự

### Bước 5: Cấu hình trong appsettings.json
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": "587",
  "SmtpUsername": "apmms.system@gmail.com",  // Email vừa tạo
  "SmtpPassword": "abcdefghijklmnop",        // App Password vừa tạo
  "FromEmail": "apmms.system@gmail.com",     // Cùng email
  "FromName": "Hệ thống APMMS"
}
```

## Lưu ý quan trọng

⚠️ **Kiểm tra email có sẵn không:**
- Gmail có thể đã có người dùng đăng ký email đó
- Nếu không được, thử các biến thể khác (thêm số, thay đổi từ)

✅ **Email khuyến nghị nhất:**
```
apmms.system@gmail.com
```
- Ngắn gọn, chuyên nghiệp
- Dễ nhớ cho người dùng
- Phù hợp với mục đích hệ thống

## Ví dụ cấu hình hoàn chỉnh

Sau khi tạo email `apmms.system@gmail.com`:

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": "587",
  "SmtpUsername": "apmms.system@gmail.com",
  "SmtpPassword": "abcd efgh ijkl mnop",
  "FromEmail": "apmms.system@gmail.com",
  "FromName": "Hệ thống APMMS"
}
```

