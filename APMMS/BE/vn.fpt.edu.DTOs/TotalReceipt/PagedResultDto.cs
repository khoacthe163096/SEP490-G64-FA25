using System.Collections.Generic;

namespace BE.vn.fpt.edu.DTOs.TotalReceipt
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
