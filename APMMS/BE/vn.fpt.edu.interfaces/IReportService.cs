namespace BE.vn.fpt.edu.interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateMaintenanceTicketPdfAsync(long maintenanceTicketId);
    }
}


