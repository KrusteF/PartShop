using System.Collections.Generic;

namespace PartShop.Models.ViewModels
{
    public class OrderHeaderOrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
