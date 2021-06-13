namespace PartShop.Models.ViewModels
{
    using System.Collections.Generic;

    public class OrderListViewModel
    {
        public IList<OrderHeaderOrderDetailsViewModel> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
