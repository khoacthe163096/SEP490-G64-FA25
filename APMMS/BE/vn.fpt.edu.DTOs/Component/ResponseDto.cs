namespace BE.vn.fpt.edu.DTOs.Component
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityStock { get; set; }
        public string? TypeComponentName { get; set; }
        public string? BranchName { get; set; }
        public string? StatusName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? PurchasePrice { get; set; }
    }
}
