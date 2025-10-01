using HotelManagementApplication.Interface.Repositories;
using HotelManagementInfratructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelManagementInfratructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly ApplicationDbContext context;
        

        private IDbContextTransaction? _transaction;

        public IBooking Booking { get; }
        public IRoom Room { get; }
        public IGuest Guest { get; }
        public IHotelStaff HotelStaff { get; }
        public IMaintenanceDate MaintenanceDate { get; }
        public IBookingOtp BookingOtp { get; }

        public UnitOfWork(ApplicationDbContext context, IBooking booking, IRoom room, IGuest guest, IHotelStaff hotelStaff, IMaintenanceDate maintenanceDate, IBookingOtp bookingOtp)
        {
            this.context = context;
            Booking = booking;
            Room = room;
            Guest = guest;
            HotelStaff = hotelStaff;
            MaintenanceDate = maintenanceDate;
            BookingOtp = bookingOtp;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await context.SaveChangesAsync();
                if (_transaction != null)
                    await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                    await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            context.Dispose();
        }
    }
}
