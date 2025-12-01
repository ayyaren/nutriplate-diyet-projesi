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

        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty; // E-posta değiştirilemez (UI'da readonly)

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Cinsiyet")]
        public string? Gender { get; set; }

        [Display(Name = "Boy (cm)")]
        [Range(50, 250, ErrorMessage = "Boy 50 ile 250 cm arasında olmalıdır.")]
        public int? HeightCm { get; set; }

        [Display(Name = "Kilo (kg)")]
        [Range(20, 300, ErrorMessage = "Kilo 20 ile 300 kg arasında olmalıdır.")]
        public double? WeightKg { get; set; }

        [Display(Name = "Aktivite Seviyesi")]
        public string? ActivityLevel { get; set; }

        [Display(Name = "Hedef Günlük Kalori (kcal)")]
        [Range(800, 4500, ErrorMessage = "Hedef kalori 800 ile 4500 kcal arasında olabilir.")]
        public int? DailyCalorieTarget { get; set; }
    }
}
