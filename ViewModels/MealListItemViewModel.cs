namespace Nutriplate.Web.ViewModels
{
    public class MealListItemViewModel
    {
        public int Id { get; set; }
        public string MealType { get; set; } = string.Empty;   // Kahvaltı, Öğle, Akşam...
        public DateTime MealDateTime { get; set; }
        public int TotalKcal { get; set; }
    }
}
