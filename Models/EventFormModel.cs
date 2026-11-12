namespace RaceDayMVC.Models
{
    public class EventFormViewModel
    {
        public string EventName { get; set; } = "";

        public string EventType { get; set; } = "";

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = "";

        public string? Description { get; set; }

        public string Status { get; set; } = "Upcoming";
    }
}
