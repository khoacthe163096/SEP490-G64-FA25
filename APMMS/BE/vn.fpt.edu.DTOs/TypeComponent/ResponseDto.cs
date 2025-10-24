namespace BE.vn.fpt.edu.DTOs.TypeComponent
{
    public class TypeComponentResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ComponentCount { get; set; }
    }

    public class TypeComponentDetailResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<ComponentInfoDto> Components { get; set; } = new List<ComponentInfoDto>();
    }

    public class ComponentInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityStock { get; set; }
        public string? ImageUrl { get; set; }
    }
}