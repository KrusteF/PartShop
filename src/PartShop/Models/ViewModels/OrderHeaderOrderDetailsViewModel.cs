namespace PartShop.Models.ViewModels
{
    using System.Collections.Generic;

    public class OrderHeaderOrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
