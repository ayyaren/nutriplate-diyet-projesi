using System;
using System.Collections.Generic;

namespace Nutriplate.Web.ViewModels
{
    public class DailySummaryViewModel
    {
        public DateTime Date { get; set; }

        public int TotalKcal { get; set; }
        public int TargetKcal { get; set; }

        public int ProteinGr { get; set; }
        public int CarbGr { get; set; }
        public int FatGr { get; set; }

        public List<MealListItemViewModel> Meals { get; set; } = new();
    }
}
