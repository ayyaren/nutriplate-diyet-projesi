using System;

namespace Nutriplate.Web.ViewModels
{
    public class MealListItemViewModel
    {
        public int Id { get; set; }

        // Kahvaltı, Öğle, Akşam, Atıştırmalık...
        public string MealType { get; set; } = string.Empty;

        public DateTime MealDateTime { get; set; }

        public int TotalKcal { get; set; }
    }
}
