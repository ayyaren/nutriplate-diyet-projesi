namespace Nutriplate.Web.ViewModels
{
    public class DietitianClientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // İstersen ileride doldururuz
        public DateTime? LastSummaryDate { get; set; }
    }
}
