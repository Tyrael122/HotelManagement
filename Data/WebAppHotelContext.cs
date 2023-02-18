using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppHotel.Models;
using WebAppHotel.Models.Entities;

namespace WebAppHotel.Data
{
    public class WebAppHotelContext : DbContext
    {
        public WebAppHotelContext (DbContextOptions<WebAppHotelContext> options)
            : base(options)
        {
        }

        public DbSet<WebAppHotel.Models.Entities.Reservation> Reservation { get; set; } = default!;

        public DbSet<WebAppHotel.Models.Entities.Customer> Customer { get; set; } = default!;

        public DbSet<WebAppHotel.Models.Entities.Room> Room { get; set; } = default!;

        public DbSet<StatusTimelineLog> statusTimelineLog { get; set; } = default!;
    }
}
