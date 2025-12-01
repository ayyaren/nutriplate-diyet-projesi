using System;
using System.Collections.Generic;

namespace Nutriplate.Web.ViewModels
{
    public class MealDetailViewModel
    {
        public int Id { get; set; }

        public string MealType { get; set; } = string.Empty;

        public DateTime MealDateTime { get; set; }

        public int TotalKcal { get; set; }

        public List<MealFoodItemViewModel> Items { get; set; } = new();

        public string? Notes { get; set; }
    }
}
