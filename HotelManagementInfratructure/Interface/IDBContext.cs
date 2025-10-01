using HootelManagementDomain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HotelManagementInfratructure.Interface
{
    public interface IDBContext
    {
        public DbSet<Booking> Bookings { get; }
        public DbSet<Guest> Guests { get; }
        public DbSet<HotelStaff> HotelStaffs { get; }
        public DbSet<Room> Rooms { get; }
        public DbSet<MaintenanceDate> MaintenanceDates { get; }
        public DbSet<BookingOtp> BookingOtps { get; }
    }
}
