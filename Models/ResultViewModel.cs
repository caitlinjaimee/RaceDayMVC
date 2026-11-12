namespace RaceDayMVC.Models
{
    public class ResultViewModel
    {
        public int ResultID { get; set; }

        public int EventID { get; set; }

        public int ParticipantID { get; set; }

        public int CategoryID { get; set; }

        public string FinishTime { get; set; } = "";

        public int Position { get; set; }
    }
}
