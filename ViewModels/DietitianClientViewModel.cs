using System;

namespace Nutriplate.Web.ViewModels
{
    public class DietitianClientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public double CurrentWeight { get; set; }
        public double TargetWeight { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
