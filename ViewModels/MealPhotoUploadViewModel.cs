using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Nutriplate.Web.ViewModels
{
    public class MealPhotoAnalysisItem
    {
        public string FoodName { get; set; } = string.Empty;
        public double Gram { get; set; }
        public int Kcal { get; set; }
    }

    public class MealPhotoUploadViewModel
    {
        [Required(ErrorMessage = "Lütfen bir fotoğraf seçin")]
        [Display(Name = "Tabak Fotoğrafı")]
        public IFormFile? Photo { get; set; }

        [Display(Name = "Notlar")]
        public string? Notes { get; set; }

        // Analiz sonucu (şimdilik dummy)
        public List<MealPhotoAnalysisItem> AnalysisResults { get; set; } = new();
        public int TotalKcal => AnalysisResults.Sum(x => x.Kcal);
    }
}
