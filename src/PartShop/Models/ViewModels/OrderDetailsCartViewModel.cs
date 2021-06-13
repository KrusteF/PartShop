namespace PartShop.Models.ViewModels
{
    using System.Collections.Generic;

    public class OrderDetailsCartViewModel
    {
        public List<ShoppingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
