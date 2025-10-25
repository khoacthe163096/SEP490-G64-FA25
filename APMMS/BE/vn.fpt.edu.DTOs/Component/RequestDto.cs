namespace BE.vn.fpt.edu.DTOs.Component
{
    public class RequestDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityStock { get; set; }
        public long? TypeComponentId { get; set; }
        public long? BranchId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}