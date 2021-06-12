using System.Collections.Generic;

namespace PartShop.Models.ViewModels
{
    public class OrderListViewModel
    {
        public IList<OrderHeaderOrderDetailsViewModel> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
