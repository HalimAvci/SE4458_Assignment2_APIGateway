namespace SE4458_midterm.Models
{
    public class FlightInsertRequest
    {
        public DateTime Date { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
        public string FlightNumber { get; set; }
    }
}
