using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;

        public CategoriesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _apiService.GetCategoriesAsync();

            return View(categories);
        }
    }
}