using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;

        public DashboardController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetEventsAsync();

            var dashboard = new DashboardViewModel
            {
                TotalEvents = events.Count,

                UpcomingEvents = events
                    .Where(e => e.EventDate >= DateTime.Today)
                    .OrderBy(e => e.EventDate)
                    .ToList()
            };

            return View(dashboard);
        }
    }
}
