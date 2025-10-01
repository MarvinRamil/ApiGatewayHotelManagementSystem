using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomImageController : ControllerBase
    {
        private readonly IRoomImageService _roomImageService;

        public RoomImageController(IRoomImageService roomImageService)
        {
            _roomImageService = roomImageService;
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetRoomImages(int roomId)
        {
            try
            {
                var images = await _roomImageService.GetRoomImagesAsync(roomId);
                return this.CreateResponse(200, "Room images retrieved successfully", images);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving room images: {ex.Message}");
            }
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetRoomImage(int imageId)
        {
            try
            {
                var image = await _roomImageService.GetRoomImageByIdAsync(imageId);
                if (image == null)
                {
                    return this.CreateResponse(404, "Image not found");
                }
                return this.CreateResponse(200, "Image retrieved successfully", image);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving image: {ex.Message}");
            }
        }

        [HttpPost("upload")]
       // [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UploadRoomImages([FromForm] RoomImageUploadDto uploadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid upload data", ModelState);
                }

                if (uploadDto.Images == null || !uploadDto.Images.Any())
                {
                    return this.CreateResponse(400, "No images provided");
                }

                // Validate each image
                foreach (var image in uploadDto.Images)
                {
                    if (!await _roomImageService.ValidateImageFileAsync(image))
                    {
                        return this.CreateResponse(400, $"Invalid image file: {image.FileName}");
                    }
                }

                var uploadedImages = await _roomImageService.UploadRoomImagesAsync(uploadDto);
                return this.CreateResponse(201, "Images uploaded successfully", uploadedImages);
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error uploading images: {ex.Message}");
            }
        }

        [HttpDelete("url")]
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteRoomImageByUrl([FromQuery] string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return this.CreateResponse(400, "Image URL is required");
                }

                var result = await _roomImageService.DeleteRoomImageByUrlAsync(imageUrl);
                if (!result)
                {
                    return this.CreateResponse(404, "Image not found");
                }
                return this.CreateResponse(200, "Image deleted successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error deleting image: {ex.Message}");
            }
        }

        [HttpPut("update")]
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateRoomImages([FromBody] RoomImageUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid update data", ModelState);
                }

                var result = await _roomImageService.UpdateRoomImagesAsync(updateDto);
                if (!result)
                {
                    return this.CreateResponse(404, "Room not found");
                }
                return this.CreateResponse(200, "Room images updated successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error updating room images: {ex.Message}");
            }
        }

        [HttpPatch("set-primary")]
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> SetPrimaryImage([FromQuery] int roomId, [FromQuery] string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return this.CreateResponse(400, "Image URL is required");
                }

                var result = await _roomImageService.SetPrimaryImageAsync(roomId, imageUrl);
                if (!result)
                {
                    return this.CreateResponse(404, "Room or image not found");
                }
                return this.CreateResponse(200, "Primary image set successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error setting primary image: {ex.Message}");
            }
        }

        [HttpPost("convert-to-webp")]
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ConvertImageToWebP([FromQuery] string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    return this.CreateResponse(400, "Image path is required");
                }

                var result = await _roomImageService.ConvertImageToWebPAsync(imagePath);
                if (!result)
                {
                    return this.CreateResponse(400, "Failed to convert image to WebP");
                }
                return this.CreateResponse(200, "Image converted to WebP successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error converting image: {ex.Message}");
            }
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidateImageFile([FromQuery] string fileName, [FromQuery] long fileSize)
        {
            try
            {
                // Create a mock IFormFile for validation
                var mockFile = new MockFormFile(fileName, fileSize);
                var isValid = await _roomImageService.ValidateImageFileAsync(mockFile);
                
                return this.CreateResponse(200, "Image validation completed", new { IsValid = isValid });
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error validating image: {ex.Message}");
            }
        }
    }

    // Helper class for validation
    public class MockFormFile : IFormFile
    {
        public MockFormFile(string fileName, long length)
        {
            FileName = fileName;
            Length = length;
        }

        public string ContentType { get; set; } = "image/jpeg";
        public string ContentDisposition { get; set; } = "";
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
        public long Length { get; set; }
        public string Name { get; set; } = "file";
        public string FileName { get; set; }

        public Stream OpenReadStream()
        {
            return new MemoryStream();
        }

        public void CopyTo(Stream target)
        {
            // Mock implementation
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}

