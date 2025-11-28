using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BE.vn.fpt.edu.DTOs.MaintenanceTicket;
using BE.vn.fpt.edu.DTOs.ServiceTask;
using BE.vn.fpt.edu.DTOs.TicketComponent;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class ReportService : IReportService
    {
        private readonly IMaintenanceTicketService _maintenanceTicketService;
        private readonly IServiceTaskService _serviceTaskService;
        private readonly ITicketComponentService _ticketComponentService;
        private readonly IHistoryLogRepository _historyLogRepository;

        public ReportService(
            IMaintenanceTicketService maintenanceTicketService,
            IServiceTaskService serviceTaskService,
            ITicketComponentService ticketComponentService,
            IHistoryLogRepository historyLogRepository)
        {
            _maintenanceTicketService = maintenanceTicketService;
            _serviceTaskService = serviceTaskService;
            _ticketComponentService = ticketComponentService;
            _historyLogRepository = historyLogRepository;
        }

        public async Task<byte[]> GenerateMaintenanceTicketPdfAsync(long maintenanceTicketId)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Lấy thông tin phiếu bảo dưỡng
            var ticket = await _maintenanceTicketService.GetMaintenanceTicketByIdAsync(maintenanceTicketId);
            if (ticket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // Lấy danh sách công việc
            var serviceTasks = await _serviceTaskService.GetServiceTasksByMaintenanceTicketIdAsync(maintenanceTicketId);
            
            // Lấy danh sách phụ tùng
            var components = await _ticketComponentService.GetByMaintenanceTicketIdAsync(maintenanceTicketId);
            
            // Lấy lịch sử thay đổi
            var historyLogs = await _historyLogRepository.GetByMaintenanceTicketIdAsync(maintenanceTicketId);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("PHIẾU BẢO DƯỠNG XE").FontSize(20).Bold();
                                    col.Item().Text($"Mã phiếu: {ticket.Code ?? "N/A"}").FontSize(12);
                                });
                                row.ConstantItem(80).AlignRight().Column(col =>
                                {
                                    col.Item().Text($"Ngày tạo: {ticket.CreatedDate:dd/MM/yyyy}").FontSize(10);
                                    if (ticket.StartTime.HasValue)
                                        col.Item().Text($"Bắt đầu: {ticket.StartTime:dd/MM/yyyy HH:mm}").FontSize(10);
                                    if (ticket.EndTime.HasValue)
                                        col.Item().Text($"Kết thúc: {ticket.EndTime:dd/MM/yyyy HH:mm}").FontSize(10);
                                });
                            });
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Thông tin khách hàng và xe
                            column.Item().PaddingBottom(10).Column(infoCol =>
                            {
                                infoCol.Item().Text("THÔNG TIN KHÁCH HÀNG VÀ XE").FontSize(14).Bold();
                                infoCol.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Element(CellStyle).Text("Khách hàng:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.CustomerName ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Số điện thoại:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.CustomerPhone ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Email:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.CustomerEmail ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Địa chỉ:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.CustomerAddress ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Biển số xe:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.LicensePlate ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Loại xe:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.VehicleType ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Mẫu xe:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.CarModel ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Số khung:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.VinNumber ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Số km:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.Mileage.HasValue ? ticket.Mileage.Value.ToString() : "N/A").FontSize(10);
                                });
                            });

                            // Thông tin phiếu bảo dưỡng
                            column.Item().PaddingBottom(10).Column(infoCol =>
                            {
                                infoCol.Item().Text("THÔNG TIN PHIẾU BẢO DƯỠNG").FontSize(14).Bold();
                                infoCol.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Element(CellStyle).Text("Trạng thái:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(GetStatusName(ticket.StatusCode)).FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Chi nhánh:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.BranchName ?? "N/A").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Tư vấn viên:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.ConsulterName ?? "N/A").FontSize(10);

                                    if (ticket.Technicians != null && ticket.Technicians.Any())
                                    {
                                        table.Cell().Element(CellStyle).Text("Kỹ thuật viên:").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(string.Join(", ", ticket.Technicians.Select(t => t.TechnicianName))).FontSize(10);
                                    }

                                    table.Cell().Element(CellStyle).Text("Mô tả:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(ticket.Description ?? "N/A").FontSize(10);
                                });
                            });

                            // Danh sách công việc
                            if (serviceTasks != null && serviceTasks.Any())
                            {
                                column.Item().PaddingBottom(10).Column(taskCol =>
                                {
                                    taskCol.Item().Text("DANH SÁCH CÔNG VIỆC").FontSize(14).Bold();
                                    taskCol.Item().PaddingTop(5).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        // Header
                                        table.Cell().Element(CellStyle).Text("Tên công việc").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("Thời gian chuẩn").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("Thời gian thực tế").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("Trạng thái").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("Phí nhân công").FontSize(10).Bold();

                                        // Rows
                                        foreach (var task in serviceTasks)
                                        {
                                            table.Cell().Element(CellStyle).Text(task.TaskName ?? "N/A").FontSize(9);
                                            table.Cell().Element(CellStyle).Text(task.StandardLaborTime.HasValue ? task.StandardLaborTime.Value.ToString("F2") + "h" : "N/A").FontSize(9);
                                            table.Cell().Element(CellStyle).Text(task.ActualLaborTime.HasValue ? task.ActualLaborTime.Value.ToString("F2") + "h" : "N/A").FontSize(9);
                                            table.Cell().Element(CellStyle).Text(GetTaskStatusName(task.StatusCode)).FontSize(9);
                                            table.Cell().Element(CellStyle).Text(task.LaborCost.HasValue ? task.LaborCost.Value.ToString("N0") + " ₫" : "N/A").FontSize(9);
                                        }
                                    });
                                });
                            }

                            // Danh sách phụ tùng
                            if (components != null && components.Any())
                            {
                                column.Item().PaddingBottom(10).Column(compCol =>
                                {
                                    compCol.Item().Text("DANH SÁCH PHỤ TÙNG").FontSize(14).Bold();
                                    compCol.Item().PaddingTop(5).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        // Header
                                        table.Cell().Element(CellStyle).Text("Tên phụ tùng").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("Số lượng").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("Số lượng thực tế").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("Đơn giá").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("Thành tiền").FontSize(10).Bold();

                                        // Rows
                                        decimal totalComponentCost = 0;
                                        foreach (var comp in components)
                                        {
                                            var quantity = comp.ActualQuantity.HasValue ? comp.ActualQuantity.Value : comp.Quantity;
                                            var unitPrice = comp.UnitPrice.HasValue ? comp.UnitPrice.Value : 0;
                                            var total = quantity * unitPrice;
                                            totalComponentCost += total;

                                            table.Cell().Element(CellStyle).Text(comp.ComponentName ?? "N/A").FontSize(9);
                                            table.Cell().Element(CellStyle).Text(comp.Quantity.ToString()).FontSize(9);
                                            table.Cell().Element(CellStyle).Text(comp.ActualQuantity.HasValue ? comp.ActualQuantity.Value.ToString() : comp.Quantity.ToString()).FontSize(9);
                                            table.Cell().Element(CellStyle).Text(unitPrice.ToString("N0") + " ₫").FontSize(9);
                                            table.Cell().Element(CellStyle).Text(total.ToString("N0") + " ₫").FontSize(9);
                                        }

                                        // Total row
                                        table.Cell().ColumnSpan(4).Element(CellStyle).Text("Tổng tiền phụ tùng:").FontSize(10).Bold().AlignRight();
                                        table.Cell().Element(CellStyle).Text(totalComponentCost.ToString("N0") + " ₫").FontSize(10).Bold();
                                    });
                                });
                            }

                            // Tổng kết chi phí
                            column.Item().PaddingBottom(10).Column(summaryCol =>
                            {
                                summaryCol.Item().Text("TỔNG KẾT CHI PHÍ").FontSize(14).Bold();
                                summaryCol.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    var totalLaborCost = serviceTasks?.Sum(t => t.LaborCost.HasValue ? t.LaborCost.Value : 0) ?? 0;
                                    var totalComponentCost = components?.Sum(c => {
                                        var qty = c.ActualQuantity.HasValue ? c.ActualQuantity.Value : c.Quantity;
                                        var price = c.UnitPrice.HasValue ? c.UnitPrice.Value : 0;
                                        return qty * price;
                                    }) ?? 0;
                                    var totalCost = totalLaborCost + totalComponentCost;

                                    table.Cell().Element(CellStyle).Text("Tổng phí nhân công:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(totalLaborCost.ToString("N0") + " ₫").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("Tổng tiền phụ tùng:").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(totalComponentCost.ToString("N0") + " ₫").FontSize(10);

                                    table.Cell().Element(CellStyle).Text("TỔNG CỘNG:").FontSize(12).Bold();
                                    table.Cell().Element(CellStyle).Text(totalCost.ToString("N0") + " ₫").FontSize(12).Bold();
                                });
                            });

                            // Lịch sử thay đổi
                            if (historyLogs != null && historyLogs.Any())
                            {
                                column.Item().PaddingBottom(10).Column(historyCol =>
                                {
                                    historyCol.Item().Text("LỊCH SỬ THAY ĐỔI").FontSize(14).Bold();
                                    historyCol.Item().PaddingTop(5).Column(logCol =>
                                    {
                                        var sortedLogs = historyLogs.OrderByDescending(l => l.CreatedAt ?? DateTime.MinValue).ToList();
                                        foreach (var log in sortedLogs)
                                        {
                                            logCol.Item().PaddingBottom(5).Column(item =>
                                            {
                                                item.Item().Row(row =>
                                                {
                                                    row.RelativeItem().Text($"{log.CreatedAt:dd/MM/yyyy HH:mm} - {GetActionName(log.Action)}").FontSize(9).Bold();
                                                });
                                                if (!string.IsNullOrEmpty(log.NewData))
                                                {
                                                    item.Item().PaddingLeft(10).Text(log.NewData).FontSize(8);
                                                }
                                                if (!string.IsNullOrEmpty(log.OldData))
                                                {
                                                    item.Item().PaddingLeft(10).Text($"Trước: {log.OldData}").FontSize(8).FontColor(Colors.Grey.Medium);
                                                }
                                            });
                                        }
                                    });
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Trang ").FontSize(9).FontColor(Colors.Grey.Medium);
                            x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                            x.Span(" / ").FontSize(9).FontColor(Colors.Grey.Medium);
                            x.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                });
            });

            return document.GeneratePdf();
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5)
                .Background(Colors.White);
        }

        private static string GetStatusName(string? statusCode)
        {
            return statusCode switch
            {
                "PENDING" => "Chờ xử lý",
                "ASSIGNED" => "Đã gán",
                "IN_PROGRESS" => "Đang thực hiện",
                "COMPLETED" => "Hoàn thành",
                "CANCELLED" => "Đã hủy",
                _ => statusCode ?? "N/A"
            };
        }

        private static string GetTaskStatusName(string? statusCode)
        {
            return statusCode switch
            {
                "PENDING" => "Chờ",
                "IN_PROGRESS" => "Đang làm",
                "DONE" => "Hoàn thành",
                "COMPLETED" => "Hoàn thành",
                "CANCELLED" => "Đã hủy",
                _ => statusCode ?? "N/A"
            };
        }

        private static string GetActionName(string? action)
        {
            return action switch
            {
                "CREATE_MAINTENANCE_TICKET" => "Tạo phiếu bảo dưỡng",
                "UPDATE_STATUS" => "Cập nhật trạng thái",
                "ASSIGN_TECHNICIAN" => "Gán kỹ thuật viên",
                "ADD_SERVICE_TASK" => "Thêm công việc",
                "UPDATE_SERVICE_TASK" => "Cập nhật công việc",
                "DELETE_SERVICE_TASK" => "Xóa công việc",
                "ADD_COMPONENT" => "Thêm phụ tùng",
                "UPDATE_COMPONENT" => "Cập nhật phụ tùng",
                "DELETE_COMPONENT" => "Xóa phụ tùng",
                "UPDATE_COMPONENT_QUANTITY" => "Cập nhật số lượng phụ tùng",
                "ASSIGN_SERVICE_TASK_TECHNICIANS" => "Gán kỹ thuật viên cho công việc",
                "WARNING_LABOR_TIME_EXCEEDED" => "Cảnh báo thời gian",
                _ => action ?? "Thay đổi"
            };
        }
    }
}
