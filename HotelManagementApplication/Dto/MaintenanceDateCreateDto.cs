using HootelManagementDomain.enums;

namespace HotelManagementApplication.Dto
{
    public class MaintenanceDateCreateDto
    {
        public DateTime Date { get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;
        public int RoomId { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
    }
}
