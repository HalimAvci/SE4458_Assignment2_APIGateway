namespace SE4458_midterm.Models
{
    public class CheckInRequest
    {
        public string FlightNumber { get; set; }
        public DateTime Date {  get; set; }
        public string PassengerName { get; set; }
    }
}
