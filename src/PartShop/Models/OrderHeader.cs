using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartShop.Models
{
    public class OrderHeader
    {
        [Key]
        [Display(Name = "Номер")]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public double OrderTotalOrignal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Обща сума")]
        public double OrderTotal { get; set; }

        [Required]
        [Display(Name = "Pickup Time")]
        public DateTime PickupTime { get; set; }

        [Display(Name = "Промо код")]
        public string CouponCode { get; set; }
        public double CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }

        [Display(Name = "Потребител")]
        public string PickupName { get; set; }

        [Display(Name = "Телефон")]
        public string PickupNumber { get; set; }

        public string TransactionId { get; set; }
    }
}
