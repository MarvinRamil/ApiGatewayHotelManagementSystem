using HootelManagementDomain.enums;

namespace HotelManagementApplication.Dto
{
    public class MaintenanceDateUpdateDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public MaintenanceStatus Status { get; set; }
        public int RoomId { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CompletedBy { get; set; }
    }
}
