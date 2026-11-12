namespace RaceDayMVC.Models
{
    public class DashboardViewModel
    {
        public int TotalEvents { get; set; }

        public List<EventViewModel> UpcomingEvents { get; set; }
            = new List<EventViewModel>();
    }
}
