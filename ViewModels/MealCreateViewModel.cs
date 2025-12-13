using System;
using System.Collections.Generic;

namespace Nutriplate.Web.ViewModels
{
    public class MealCreateViewModel
    {
        public string MealType { get; set; } = string.Empty;

        // Öğünün tarihi / saati
        public DateTime MealDateTime { get; set; } = DateTime.Now;

        // Toplam kalori (kullanıcı elle girebilir, opsiyonel)
        public int? TotalKcal { get; set; }

        // Not (opsiyonel)
        public string? Notes { get; set; }

        // Dinamik eklediğimiz besin satırları
        public List<MealFoodItemViewModel> Items { get; set; } = new();
    }
}
