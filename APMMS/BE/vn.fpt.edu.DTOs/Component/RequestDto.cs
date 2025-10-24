using System.ComponentModel.DataAnnotations;

namespace BE.vn.fpt.edu.DTOs.Component
{
    public class CreateComponentDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        public string? Code { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be positive")]
        public decimal? UnitPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity stock must be non-negative")]
        public int? QuantityStock { get; set; }

        public long? TypeComponentId { get; set; }
        public long? BranchId { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }
    }

    public class UpdateComponentDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        public string? Code { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be positive")]
        public decimal? UnitPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity stock must be non-negative")]
        public int? QuantityStock { get; set; }

        public long? TypeComponentId { get; set; }
        public long? BranchId { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }
    }

    public class UpdateStockDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Quantity stock must be non-negative")]
        public int? QuantityStock { get; set; }
    }
}