namespace HotelManagementApplication.Interface.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBooking Booking { get; }
        IRoom Room { get; }
        IGuest Guest { get; }
        IHotelStaff HotelStaff { get; }
        IMaintenanceDate MaintenanceDate { get; }
        IBookingOtp BookingOtp { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task SaveAsync();
        Task Save();
    }
}
