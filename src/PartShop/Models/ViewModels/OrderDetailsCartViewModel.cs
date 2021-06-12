using System.Collections.Generic;

namespace PartShop.Models.ViewModels
{
    public class OrderDetailsCartViewModel
    {
        public List<ShoppingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
