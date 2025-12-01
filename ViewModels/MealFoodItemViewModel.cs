namespace Nutriplate.Web.ViewModels
{
    public class MealFoodItemViewModel
    {
        public string FoodName { get; set; } = string.Empty;
        public double Gram { get; set; }
        public int Kcal { get; set; }

        // İstersen ileride ekleyebiliriz:
        // public double ProteinGr { get; set; }
        // public double CarbGr { get; set; }
        // public double FatGr { get; set; }
    }
}
