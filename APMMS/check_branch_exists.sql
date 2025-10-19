-- Kiểm tra Branch với id = 1
SELECT id, name, address, phone, email, created_at
FROM branch 
WHERE id = 1;

-- Kiểm tra tất cả Branch
SELECT id, name, address, phone, email, created_at
FROM branch 
ORDER BY id;

-- Kiểm tra VehicleCheckin với branch_id = 1
SELECT vc.id, vc.branch_id, vc.car_id, b.name as branch_name
FROM vehicle_checkin vc
LEFT JOIN branch b ON vc.branch_id = b.id
WHERE vc.id = 7;
