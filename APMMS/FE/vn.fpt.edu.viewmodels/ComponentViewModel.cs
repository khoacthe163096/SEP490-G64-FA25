using System.Collections.Generic;

namespace vn.fpt.edu.viewmodels
{
    public class ComponentViewModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityStock { get; set; }
        public long? TypeComponentId { get; set; }
        public string? TypeComponentName { get; set; }
        public long? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? ImageUrl { get; set; }
        public string? StatusCode { get; set; }
        public decimal? PurchasePrice { get; set; }
    }

    public class ComponentIndexViewModel
    {
        public IEnumerable<ComponentViewModel> Items { get; set; } = new List<ComponentViewModel>();
        public IEnumerable<TypeComponentLookupViewModel> TypeComponents { get; set; } = new List<TypeComponentLookupViewModel>();
        public string? Search { get; set; }
        public long? BranchId { get; set; }
        public long? TypeComponentFilterId { get; set; }
        public string? StatusCode { get; set; }
    }

    public class TypeComponentLookupViewModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }
}
