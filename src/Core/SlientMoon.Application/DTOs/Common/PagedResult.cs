using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Common
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public PageMeta Meta { get; set; } = null!;
    }

    public class PageMeta
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
    }
}
