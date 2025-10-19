-- Cập nhật status_code cho các VehicleCheckin chưa có status
UPDATE vehicle_checkin 
SET status_code = 'PENDING' 
WHERE status_code IS NULL;

-- Kiểm tra kết quả
SELECT id, status_code, created_at 
FROM vehicle_checkin 
ORDER BY created_at DESC;

