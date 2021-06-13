namespace PartShop.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [Display(Name = "Адрес")]
        public string StreetAddress { get; set; }

        [Display(Name = "Град")]
        public string City { get; set; }

        [Display(Name = "Държава")]
        public string State { get; set; }

        [Display(Name = "Пощенски код")]
        public string PostalCode { get; set; }
    }
}
