using System;

namespace PartShop.Models
{
    public class PagingInfo
    {
        public int TotalItem { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string UrlParam { get; set; }
    }
}
