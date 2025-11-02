-- Thêm dữ liệu hình ảnh mẫu cho VehicleCheckin
INSERT INTO vehicle_checkin_image (vehicle_checkin_id, image_url, created_at)
VALUES 
(1, 'https://picsum.photos/400/300?random=1', GETDATE()),
(1, 'https://picsum.photos/400/300?random=2', GETDATE()),
(2, 'https://picsum.photos/400/300?random=3', GETDATE()),
(3, 'https://picsum.photos/400/300?random=4', GETDATE()),
(4, 'https://picsum.photos/400/300?random=5', GETDATE()),
(5, 'https://picsum.photos/400/300?random=6', GETDATE()),
(6, 'https://picsum.photos/400/300?random=7', GETDATE()),
(7, 'https://picsum.photos/400/300?random=8', GETDATE());

-- Kiểm tra dữ liệu
SELECT vci.id, vci.vehicle_checkin_id, vci.image_url, vci.created_at
FROM vehicle_checkin_image vci
ORDER BY vci.created_at DESC;
