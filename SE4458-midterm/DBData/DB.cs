namespace SE4458_midterm.DBData
{
    using Microsoft.EntityFrameworkCore;
    using SE4458_midterm.Models;
    using System.Net.Sockets;
    public class DBData : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<Flight> Flights { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public DBData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }


}
