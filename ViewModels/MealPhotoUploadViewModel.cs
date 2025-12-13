using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Nutriplate.Web.ViewModels
{
    public class MealPhotoUploadViewModel
    {
        // Yüklenecek fotoğraf
        public IFormFile? Photo { get; set; }

        // Öğün tipi (Kahvaltı / Öğle / Akşam / Atıştırmalık)
        public string MealType { get; set; } = string.Empty;

        // Bu öğünün tarihi ve saati
        public DateTime MealDateTime { get; set; } = DateTime.Now;

        // Kullanıcının ek notu (opsiyonel)
        public string? Notes { get; set; }

        // Fotoğraftan çıkan analiz sonucu (yiyecek listesi)
        public List<MealFoodItemViewModel> AnalysisResults { get; set; } = new();
    }
}
