
namespace SE4458_midterm.Controllers
{
    using SE4458_midterm.Models;
    using Microsoft.AspNetCore.Mvc;
    using SE4458_midterm.DBData;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly DBData _dbData;

        public TicketController(DBData dbData)
        {
            _dbData = dbData;
        }

        [HttpGet("mobileApp/query")]
        public IActionResult QueryTicket(DateTime date, string from, string to, int numberOfPeople, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Page number and page size must be greater than 0.");
                }

                
                var totalFlights = _dbData.Flights
                    .Where(f => f.Date.Date == date.Date && f.FromLoc == from && f.ToLoc == to && f.AvailableSeats >= numberOfPeople)
                    .Count();

               
                var flights = _dbData.Flights
                    .Where(f => f.Date.Date == date.Date && f.FromLoc == from && f.ToLoc == to && f.AvailableSeats >= numberOfPeople)
                    .OrderBy(f => f.Date)
                    .Skip((pageNumber - 1) * pageSize) 
                    .Take(pageSize)
                    .ToList();

                
                return Ok(new
                {
                    Flights = flights,
                    TotalFlights = totalFlights,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpPost("mobileApp/buy")]
        public IActionResult BuyTicket([FromBody] TicketPurchaseRequest request)
        {
            try
            {
                var flight = _dbData.Flights
                    .FirstOrDefault(f => f.Date == request.Date && f.FromLoc == request.From && f.ToLoc == request.To && f.AvailableSeats > 0);

                if (flight == null)
                {
                    return BadRequest("No available flights for the specified criteria.");
                }

                flight.AvailableSeats--;

                var ticket = new Ticket
                {
                    Date = request.Date,
                    FromLoc = request.From,
                    ToLoc = request.To,
                    PassName = request.PassengerName,
                    NoOfPeople = 1,
                    FlightNumber = flight.FlightNumber
                };

                _dbData.Tickets.Add(ticket);
                _dbData.SaveChanges();

                
                var bookedTicket = _dbData.Tickets
                    .Where(t => t.Date == request.Date && t.FromLoc == request.From && t.ToLoc == request.To && t.PassName == request.PassengerName)
                    .FirstOrDefault();

                return Ok(new { Status = "Ticket booked successfully.", FlightDetails = flight, BookedTicket = bookedTicket });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Error saving changes to the database: {ex.InnerException?.Message}");
            }
        }

        [HttpPost("mobileApp/checkin")]
        public IActionResult CheckIn([FromBody] CheckInRequest request)
        {
            try
            {
                var flight = _dbData.Flights
                    .FirstOrDefault(f => f.FlightNumber == request.FlightNumber && f.Date == request.Date);

                if (flight == null)
                {
                    return NotFound("Flight not found.");
                }

                var ticket = _dbData.Tickets
                    .FirstOrDefault(t => t.PassName == request.PassengerName && t.FlightNumber == request.FlightNumber);

                if (ticket == null)
                {
                    return BadRequest("Ticket not found.");
                }

                if (ticket.IsCheckedIn)
                {
                    return BadRequest("Passenger has already checked in.");
                }

                ticket.IsCheckedIn = true;
                _dbData.SaveChanges();

                return Ok(new { Status = "Check-in completed successfully.", Ticket = ticket });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("Admin/insertFlight")]
        [Authorize]
        public IActionResult InsertFlight([FromBody] FlightInsertRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request data is null.");
                }

                var flight = new Flight
                {
                    Date = request.Date,
                    FromLoc = request.FromLoc,
                    ToLoc = request.ToLoc,
                    AvailableSeats = request.AvailableSeats,
                    Price = request.Price,
                    FlightNumber = request.FlightNumber
                };

                _dbData.Flights.Add(flight);
                _dbData.SaveChanges();

                var insertedFlight = _dbData.Flights.FirstOrDefault(f => f.ID == flight.ID);

                return Ok(new { Status = "Flight added successfully.", Flight = insertedFlight });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error inserting flight: {ex.Message} InnerException: {ex.InnerException?.Message}");
            }

        }

        [HttpPost("Admin/reportFlights")]
        [Authorize]
        public IActionResult ReportFlights([FromBody] FlightReportRequest request, int pageNumber = 1, int pageSize = 10)
        {
            try
            {

                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Page number and page size must be greater than 0.");
                }

                var flightsQuery = _dbData.Flights.AsQueryable();

                if (request.Date.HasValue)
                {
                    flightsQuery = flightsQuery.Where(f => f.Date.Date == request.Date.Value.Date);
                }

                if (!string.IsNullOrEmpty(request.FromLoc))
                {
                    flightsQuery = flightsQuery.Where(f => f.FromLoc.Contains(request.FromLoc));
                }

                if (!string.IsNullOrEmpty(request.ToLoc))
                {
                    flightsQuery = flightsQuery.Where(f => f.ToLoc.Contains(request.ToLoc));
                }

                if (request.AvailableSeats.HasValue)
                {
                    flightsQuery = flightsQuery.Where(f => f.AvailableSeats >= request.AvailableSeats.Value);
                }

                if (request.MinPrice.HasValue)
                {
                    flightsQuery = flightsQuery.Where(f => f.Price >= request.MinPrice.Value);
                }

                if (request.MaxPrice.HasValue)
                {
                    flightsQuery = flightsQuery.Where(f => f.Price <= request.MaxPrice.Value);
                }

                var totalFlights = flightsQuery.Count();
                var totalPages = (int)Math.Ceiling(totalFlights / (double)pageSize);

                var flights = flightsQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(f => f.Date)
                    .ToList();

                return Ok(new
                {
                    Status = "Report generated successfully.",
                    Flights = flights,
                    TotalFlights = totalFlights,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating flight report: {ex.Message}");
            }
        }

        }
    }
