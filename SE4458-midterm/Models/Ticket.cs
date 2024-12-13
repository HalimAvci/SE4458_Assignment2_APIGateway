namespace SE4458_midterm.Models
{
    public class Ticket
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public string PassName { get; set; }
        public int NoOfPeople { get; set; }
        public string FlightNumber { get; set; }
        public bool IsCheckedIn { get; set; } = false;

    }
}
