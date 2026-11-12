
namespace RaceDayAPI.Models
{
    public class Route
    {
        public int RouteID { get; set; }
        public int EventID { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public int? ElevationGain { get; set; }
        public string? MapUrl { get; set; }

        public Event? Event { get; set; }
    }
}



