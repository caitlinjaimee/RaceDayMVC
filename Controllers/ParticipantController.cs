using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;
using RaceDayMVC.Models;

namespace RaceDayMVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetEventsAsync();

            var upcomingEvents = events
                .Where(e => e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToList();

            return View(upcomingEvents);
        }

        public async Task<IActionResult> Details(int id)
        {
            var events = await _apiService.GetEventsAsync();

            var selectedEvent = events.FirstOrDefault(
                e => e.EventID == id);

            if (selectedEvent == null)
            {
                return NotFound();
            }

            return View(selectedEvent);
        }

        public IActionResult Enrollments()
        {
            return View(new List<EnrollmentViewModel>());
        }

        public IActionResult Results()
        {
            return View(new List<ResultViewModel>());
        }
    }
}