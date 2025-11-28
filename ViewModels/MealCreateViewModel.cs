using System.ComponentModel.DataAnnotations;

namespace Nutriplate.Web.ViewModels
{
    public class MealCreateViewModel
    {
        [Required(ErrorMessage = "Öğün türü zorunludur")]
        public string MealType { get; set; } = string.Empty; // Kahvaltı, Öğle vb.

        [Required(ErrorMessage = "Tarih / saat zorunludur")]
        [DataType(DataType.DateTime)]
        public DateTime MealDateTime { get; set; }

        [Required(ErrorMessage = "Toplam kalori zorunludur")]
        [Range(0, 10000, ErrorMessage = "Kalori 0 ile 10000 arasında olmalıdır")]
        public int TotalKcal { get; set; }

        [Display(Name = "Notlar / Açıklama")]
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
