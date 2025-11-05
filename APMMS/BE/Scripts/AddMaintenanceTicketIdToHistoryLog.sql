-- Script để thêm cột maintenance_ticket_id vào bảng history_log
-- Chạy script này trên SQL Server

USE [YourDatabaseName]; -- Thay đổi tên database của bạn
GO

-- Kiểm tra xem cột đã tồn tại chưa
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('history_log') 
    AND name = 'maintenance_ticket_id'
)
BEGIN
    -- Thêm cột maintenance_ticket_id
    ALTER TABLE history_log
    ADD maintenance_ticket_id BIGINT NULL;
    
    PRINT 'Đã thêm cột maintenance_ticket_id vào bảng history_log';
END
ELSE
BEGIN
    PRINT 'Cột maintenance_ticket_id đã tồn tại trong bảng history_log';
END
GO

-- Thêm foreign key constraint (tùy chọn - nếu muốn đảm bảo referential integrity)
IF NOT EXISTS (
    SELECT 1 
    FROM sys.foreign_keys 
    WHERE name = 'FK_history_log_maintenance_ticket'
)
BEGIN
    ALTER TABLE history_log
    ADD CONSTRAINT FK_history_log_maintenance_ticket
    FOREIGN KEY (maintenance_ticket_id) 
    REFERENCES maintenance_ticket(id);
    
    PRINT 'Đã thêm foreign key constraint FK_history_log_maintenance_ticket';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint FK_history_log_maintenance_ticket đã tồn tại';
END
GO

-- Tạo index để tăng tốc độ query (tùy chọn)
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_history_log_maintenance_ticket_id'
)
BEGIN
    CREATE INDEX IX_history_log_maintenance_ticket_id
    ON history_log(maintenance_ticket_id);
    
    PRINT 'Đã tạo index IX_history_log_maintenance_ticket_id';
END
ELSE
BEGIN
    PRINT 'Index IX_history_log_maintenance_ticket_id đã tồn tại';
END
GO

PRINT 'Hoàn thành!';
GO

