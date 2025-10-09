-- Tạo user admin với RoleId = 1 (Admin)
INSERT INTO ApplicationUsers (UserName, PasswordHash, Email, PhoneNumber, RoleId)
VALUES ('admin', 'admin123', 'admin@apmms.com', '0123456789', 1);

-- Kiểm tra user đã được tạo
SELECT Id, UserName, RoleId, Email FROM ApplicationUsers WHERE UserName = 'admin';
