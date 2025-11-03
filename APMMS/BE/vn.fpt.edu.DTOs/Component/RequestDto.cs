namespace BE.vn.fpt.edu.DTOs.Component
{
    public class RequestDto
    {
        public long? Id { get; set; } // null => create
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public int? QuantityStock { get; set; }
        public long? BranchId { get; set; }
        public long? TypeComponentId { get; set; }
        public string? StatusCode { get; set; }
    }
}