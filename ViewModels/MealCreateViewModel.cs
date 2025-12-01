using System;
using System.ComponentModel.DataAnnotations;

namespace Nutriplate.Web.ViewModels
{
    public class MealCreateViewModel
    {
        [Required(ErrorMessage = "Öğün türü zorunludur")]
        [Display(Name = "Öğün Türü")]
        public string MealType { get; set; } = "Kahvaltı"; // Varsayılan

        [Required(ErrorMessage = "Tarih / saat zorunludur")]
        [Display(Name = "Tarih / Saat")]
        [DataType(DataType.DateTime)]
        public DateTime MealDateTime { get; set; } = DateTime.Now;

        [Display(Name = "Toplam Kalori (kcal)")]
        [Range(0, 10000, ErrorMessage = "Kalori 0 ile 10000 arasında olmalıdır")]
        public int? TotalKcal { get; set; }  // Zorunlu değil → Fotoğraf analizinden veya manuel eklemeden gelecek.

        [Display(Name = "Notlar / Açıklama")]
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
