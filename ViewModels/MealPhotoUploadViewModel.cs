using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Nutriplate.Web.ViewModels
{
    public class MealPhotoUploadViewModel
    {
        [Required(ErrorMessage = "Lütfen bir fotoğraf seçin.")]
        [Display(Name = "Öğün Fotoğrafı")]
        public IFormFile? Photo { get; set; }

        [Display(Name = "Notlar")]
        [StringLength(250)]
        public string? Notes { get; set; }

        [Display(Name = "Tahmini Toplam Kalori (kcal)")]
        public int? TotalKcal { get; set; }

        // Fotoğraftan çıkan yiyecek listesi
        public List<MealFoodItemViewModel>? AnalysisResults { get; set; }

        // Hata mesajı (fotoğraf seçilmedi vs.)
        public string? ErrorMessage { get; set; }
    }
}
