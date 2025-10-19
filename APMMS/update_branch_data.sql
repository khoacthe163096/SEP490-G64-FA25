-- Kiểm tra dữ liệu hiện tại
SELECT vc.id, vc.branch_id, vc.car_id, c.branch_id as car_branch_id, 
       b1.name as vehicle_checkin_branch, b2.name as car_branch
FROM vehicle_checkin vc
LEFT JOIN car c ON vc.car_id = c.id
LEFT JOIN branch b1 ON vc.branch_id = b1.id
LEFT JOIN branch b2 ON c.branch_id = b2.id
ORDER BY vc.id;

-- Cập nhật branch_id cho vehicle_checkin từ car.branch_id nếu chưa có
UPDATE vehicle_checkin 
SET branch_id = c.branch_id
FROM vehicle_checkin vc
INNER JOIN car c ON vc.car_id = c.id
WHERE vc.branch_id IS NULL AND c.branch_id IS NOT NULL;

-- Kiểm tra lại sau khi cập nhật
SELECT vc.id, vc.branch_id, vc.car_id, c.branch_id as car_branch_id, 
       b1.name as vehicle_checkin_branch, b2.name as car_branch
FROM vehicle_checkin vc
LEFT JOIN car c ON vc.car_id = c.id
LEFT JOIN branch b1 ON vc.branch_id = b1.id
LEFT JOIN branch b2 ON c.branch_id = b2.id
ORDER BY vc.id;
