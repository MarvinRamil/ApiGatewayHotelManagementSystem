using Microsoft.AspNetCore.Mvc;

namespace BookingServiceApi.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult CreateResponse(this ControllerBase controller, int statusCode, string message, object? data = null)
        {
            var response = new
            {
                Success = statusCode >= 200 && statusCode < 300,
                Message = message,
                Data = data,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow
            };

            return controller.StatusCode(statusCode, response);
        }

        public static IActionResult CreateResponse(this ControllerBase controller, int statusCode, string message)
        {
            return controller.CreateResponse(statusCode, message, null);
        }
    }
}
