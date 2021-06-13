namespace PartShop.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [Required]
        public string CouponType { get; set; }
        public enum ECouponType { Процент = 0, Сума = 1 }

        [Required]
        [Display(Name = "Отстъпка")]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Минимална стойност")]
        public double MinimumAmount { get; set; }
        public byte[] Picture { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; }

    }
}
