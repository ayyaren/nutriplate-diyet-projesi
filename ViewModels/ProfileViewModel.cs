using System;
using System.ComponentModel.DataAnnotations;

namespace Nutriplate.Web.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Ad Soyad")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "E-posta")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // genelde değiştirmeyiz

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Cinsiyet")]
        public string? Gender { get; set; }

        [Display(Name = "Boy (cm)")]
        [Range(50, 250)]
        public int? HeightCm { get; set; }

        [Display(Name = "Kilo (kg)")]
        [Range(20, 300)]
        public double? WeightKg { get; set; }

        [Display(Name = "Aktivite Seviyesi")]
        public string? ActivityLevel { get; set; }
    }
}
