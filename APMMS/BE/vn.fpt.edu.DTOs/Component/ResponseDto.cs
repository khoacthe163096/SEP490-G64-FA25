namespace BE.vn.fpt.edu.DTOs.Component
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal? UnitPrice { get; set; }
        public int QuantityStock { get; set; }
        public long? TypeComponentId { get; set; }
        public string TypeComponentName { get; set; } // optional for include
        public long? BranchId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}