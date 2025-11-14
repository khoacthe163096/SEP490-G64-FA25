using System.Collections.Generic;

namespace vn.fpt.edu.viewmodels
{
    public class ServicePackageViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public string? Code { get; set; }

        public long? BranchId { get; set; }

        public string? BranchName { get; set; }

        public string? StatusCode { get; set; }

        public string? StatusName { get; set; }
    }

    public class ServicePackageIndexViewModel
    {
        public IEnumerable<ServicePackageViewModel> Items { get; set; } = new List<ServicePackageViewModel>();

        public string? Search { get; set; }

        public string? StatusCode { get; set; }
    }
}
