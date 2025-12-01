using System;

namespace Nutriplate.Web.ViewModels
{
    public class DailySummaryBoxViewModel
    {
        public DateTime Date { get; set; }

        public int TotalKcal { get; set; }
        public int TargetKcal { get; set; }
        public int RemainingKcal => TargetKcal - TotalKcal;

        public int ProteinGr { get; set; }
        public int CarbGr { get; set; }
        public int FatGr { get; set; }
    }
}
