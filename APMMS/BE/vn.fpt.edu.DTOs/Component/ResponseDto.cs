namespace BE.vn.fpt.edu.DTOs.Component
{
    public class ComponentResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityStock { get; set; }
        public string? ImageUrl { get; set; }
        public long? TypeComponentId { get; set; }
        public string? TypeComponentName { get; set; }
        public long? BranchId { get; set; }
        public string? BranchName { get; set; }
    }

    public class ComponentListResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityStock { get; set; }
        public string? ImageUrl { get; set; }
        public long? TypeComponentId { get; set; }
        public string? TypeComponentName { get; set; }
        public long? BranchId { get; set; }
        public string? BranchName { get; set; }
    }

    public class PaginationDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}