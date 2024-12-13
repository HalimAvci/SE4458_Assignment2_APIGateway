namespace SE4458_midterm.Models
{
    public class FlightReportRequest
    {
        public DateTime? Date { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public int? AvailableSeats { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
