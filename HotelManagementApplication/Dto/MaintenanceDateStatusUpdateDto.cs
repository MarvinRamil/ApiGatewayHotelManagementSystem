using HootelManagementDomain.enums;

namespace HotelManagementApplication.Dto
{
    public class MaintenanceDateStatusUpdateDto
    {
        public MaintenanceStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? CompletedBy { get; set; }
    }
}
