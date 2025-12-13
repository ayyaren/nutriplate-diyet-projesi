using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Services
{
    public interface IMealService
    {
        // Mevcutlar (dokunmuyoruz)
        Task<IReadOnlyList<MealListItemViewModel>> GetMealsForUserAsync(int userId, string jwtToken);
        Task<MealDetailViewModel?> GetMealAsync(int mealId, string jwtToken);
        Task<bool> CreateMealAsync(MealCreateViewModel model, string jwtToken);
        Task<bool> DeleteMealAsync(int mealId, string jwtToken);

        // ✅ Yeni: Seçili gün için aktif kullanıcının öğünleri
        // → GET /api/meals/day?date=YYYY-MM-DD
        Task<IReadOnlyList<MealListItemViewModel>> GetMealsForDayAsync(DateTime date, string jwtToken);

        // ✅ Yeni: Seçili gün için aktif kullanıcının günlük özeti
        // → GET /api/meals/day-summary?date=YYYY-MM-DD
        Task<DailySummaryViewModel?> GetDailySummaryForDayAsync(DateTime date, string jwtToken);
    }
}
