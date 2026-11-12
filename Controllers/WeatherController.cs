using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public WeatherController(RaceDayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather()
        {
            var weather = await _context.Weather.ToListAsync();

            return Ok(weather);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetWeatherByEvent(int eventId)
        {
            var weather = await _context.Weather
                .FirstOrDefaultAsync(w => w.EventID == eventId);

            if (weather == null)
            {
                return NotFound("Weather not found for this event.");
            }

            return Ok(weather);
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> AddWeather(Weather weather)
        {
            var eventItem = await _context.Events
                .FindAsync(weather.EventID);

            if (eventItem == null)
            {
                return BadRequest("Event not found.");
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId) ||
                eventItem.OrganiserID != organiserId)
            {
                return Forbid();
            }

            bool weatherExists = await _context.Weather
                .AnyAsync(w => w.EventID == weather.EventID);

            if (weatherExists)
            {
                return BadRequest(
                    "Weather already exists for this event.");
            }

            int nextWeatherID = (await _context.Weather
                .MaxAsync(w => (int?)w.WeatherID) ?? 0) + 1;

            weather.WeatherID = nextWeatherID;

            _context.Weather.Add(weather);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetWeatherByEvent),
                new { eventId = weather.EventID },
                weather);
        }
    }
}
