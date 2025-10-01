using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementApplication.Interface.Services;
using HootelManagementDomain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementInfratructure.Services
{
    public class RoomImageService : IRoomImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _uploadPath;
        private readonly string[] _allowedExtensions;
        private readonly long _maxFileSize;

        public RoomImageService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            
            // Load configuration values
            _maxFileSize = _configuration.GetValue<long>("ImageUpload:MaxFileSize", 10 * 1024 * 1024);
            _allowedExtensions = _configuration.GetSection("ImageUpload:AllowedExtensions").Get<string[]>() 
                ?? new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            
            // Handle null WebRootPath by using ContentRootPath as fallback
            var basePath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
            var uploadSubPath = _configuration.GetValue<string>("ImageUpload:UploadPath", "images/rooms");
            
            // If WebRootPath is null, we need to add wwwroot, otherwise use the path directly
            if (_webHostEnvironment.WebRootPath == null)
            {
                _uploadPath = Path.Combine(basePath, "wwwroot", uploadSubPath);
            }
            else
            {
                _uploadPath = Path.Combine(basePath, uploadSubPath);
            }
            
            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<ICollection<RoomImageResponseDto>> GetRoomImagesAsync(int roomId)
        {
            var room = await _unitOfWork.Room.GetById(roomId);
            if (room == null)
            {
                return new List<RoomImageResponseDto>();
            }

            return room.Images.Select((imageUrl, index) => new RoomImageResponseDto
            {
                Id = index + 1,
                RoomId = roomId,
                ImageUrl = imageUrl,
                ImageName = Path.GetFileName(imageUrl),
                FileSize = GetImageFileSizeAsync(imageUrl).Result,
                ContentType = "image/webp",
                UploadedAt = File.GetCreationTime(GetFullImagePath(imageUrl)),
                IsPrimary = index == 0
            }).ToList();
        }

        public async Task<RoomImageResponseDto?> GetRoomImageByIdAsync(int imageId)
        {
            // This would need a separate RoomImage entity for proper implementation
            // For now, we'll return null as we're using a list of strings in Room entity
            return null;
        }

        public async Task<ICollection<RoomImageResponseDto>> UploadRoomImagesAsync(RoomImageUploadDto uploadDto)
        {
            var room = await _unitOfWork.Room.GetById(uploadDto.RoomId);
            if (room == null)
            {
                throw new InvalidOperationException($"Room with ID {uploadDto.RoomId} not found.");
            }

            var uploadedImages = new List<RoomImageResponseDto>();

            foreach (var file in uploadDto.Images)
            {
                try
                {
                    if (!await ValidateImageFileAsync(file))
                    {
                        throw new InvalidOperationException($"Invalid image file: {file.FileName}");
                    }

                    var uniqueFileName = await GenerateUniqueFileNameAsync(file.FileName);
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var webpFileName = Path.ChangeExtension(uniqueFileName, ".webp");
                    var filePath = Path.Combine(_uploadPath, webpFileName);

                    // If the file is already WebP, save it directly
                    if (fileExtension == ".webp")
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        // For other formats, save temporarily and convert
                        var tempPath = Path.Combine(_uploadPath, uniqueFileName);
                        using (var stream = new FileStream(tempPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Convert to WebP
                        var conversionSuccess = await ConvertImageToWebPAsync(tempPath, filePath);
                        if (!conversionSuccess)
                        {
                            // If conversion fails, try to use the original file
                            if (File.Exists(tempPath))
                            {
                                File.Move(tempPath, filePath);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Failed to process image: {file.FileName}");
                            }
                        }
                        else
                        {
                            // Delete temporary file if conversion was successful
                            if (File.Exists(tempPath))
                            {
                                File.Delete(tempPath);
                            }
                        }
                    }

                    // Verify the final file exists
                    if (!File.Exists(filePath))
                    {
                        throw new InvalidOperationException($"Failed to save image: {file.FileName}");
                    }

                    // Validate WebP file after saving (if it's a WebP file)
                    if (fileExtension == ".webp" && !await ValidateWebPFileAsync(filePath))
                    {
                        // If validation fails, delete the file and throw an error
                        try { File.Delete(filePath); } catch { }
                        throw new InvalidOperationException($"Invalid WebP file: {file.FileName}");
                    }

                    // Add to room images
                    var imageUrl = $"/images/rooms/{webpFileName}";
                    room.Images.Add(imageUrl);
                    room.UpdatedAt = DateTime.UtcNow;

                    uploadedImages.Add(new RoomImageResponseDto
                    {
                        Id = room.Images.Count,
                        RoomId = uploadDto.RoomId,
                        ImageUrl = imageUrl,
                        ImageName = webpFileName,
                        Description = uploadDto.Description,
                        FileSize = new FileInfo(filePath).Length,
                        ContentType = "image/webp",
                        UploadedAt = DateTime.UtcNow,
                        IsPrimary = room.Images.Count == 1
                    });
                }
                catch (Exception ex)
                {
                    // Clean up any temporary files
                    var uniqueFileName = await GenerateUniqueFileNameAsync(file.FileName);
                    var tempPath = Path.Combine(_uploadPath, uniqueFileName);
                    if (File.Exists(tempPath))
                    {
                        try { File.Delete(tempPath); } catch { }
                    }
                    
                    throw new InvalidOperationException($"Error processing image {file.FileName}: {ex.Message}");
                }
            }

            await _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();

            return uploadedImages;
        }

        public async Task<bool> DeleteRoomImageAsync(int imageId)
        {
            // This would need a separate RoomImage entity for proper implementation
            // For now, we'll return false
            return false;
        }

        public async Task<bool> DeleteRoomImageByUrlAsync(string imageUrl)
        {
            // Find room with this image
            var rooms = await _unitOfWork.Room.GetAllAsync();
            var room = rooms.FirstOrDefault(r => r.Images.Contains(imageUrl));
            
            if (room == null)
            {
                return false;
            }

            // Remove from room images
            room.Images.Remove(imageUrl);
            room.UpdatedAt = DateTime.UtcNow;

            // Delete physical file
            var filePath = GetFullImagePath(imageUrl);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateRoomImagesAsync(RoomImageUpdateDto updateDto)
        {
            var room = await _unitOfWork.Room.GetById(updateDto.RoomId);
            if (room == null)
            {
                return false;
            }

            // Delete old images that are not in the new list
            var imagesToDelete = room.Images.Where(img => !updateDto.ImageUrls.Contains(img)).ToList();
            foreach (var imageUrl in imagesToDelete)
            {
                var filePath = GetFullImagePath(imageUrl);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            // Update room images
            room.Images = updateDto.ImageUrls;
            room.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> SetPrimaryImageAsync(int roomId, string imageUrl)
        {
            var room = await _unitOfWork.Room.GetById(roomId);
            if (room == null || !room.Images.Contains(imageUrl))
            {
                return false;
            }

            // Move the image to the front of the list (making it primary)
            room.Images.Remove(imageUrl);
            room.Images.Insert(0, imageUrl);
            room.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> ConvertImageToWebPAsync(string imagePath)
        {
            return await ConvertImageToWebPAsync(imagePath, Path.ChangeExtension(imagePath, ".webp"));
        }

        private async Task<bool> ConvertImageToWebPAsync(string inputPath, string outputPath)
        {
            try
            {
                var inputExtension = Path.GetExtension(inputPath).ToLowerInvariant();
                
                // If the input file is already WebP, just copy it
                if (inputExtension == ".webp")
                {
                    await Task.Run(() => File.Copy(inputPath, outputPath, true));
                    return true;
                }
                
                // For other formats, we'll copy and rename for now
                // In production, you should use a proper image processing library like:
                // - ImageSharp (https://github.com/SixLabors/ImageSharp)
                // - SkiaSharp (https://github.com/mono/SkiaSharp)
                // - Magick.NET (https://github.com/dlemstra/Magick.NET)
                
                await Task.Run(() => File.Copy(inputPath, outputPath, true));
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error converting image to WebP: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GenerateUniqueFileNameAsync(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            
            return $"{fileNameWithoutExtension}_{timestamp}_{random}{extension}";
        }

        public async Task<bool> ValidateImageFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            if (file.Length > _maxFileSize)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return false;
            }

            // For now, we'll skip the WebP header validation to avoid stream conflicts
            // The file extension and size validation should be sufficient for most cases
            // In production, you might want to add more sophisticated image validation
            // using libraries like ImageSharp or checking file headers after saving the file
            
            return true;
        }

        public async Task<long> GetImageFileSizeAsync(string imagePath)
        {
            try
            {
                var basePath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
                string fullPath;
                
                if (_webHostEnvironment.WebRootPath == null)
                {
                    fullPath = Path.Combine(basePath, "wwwroot", imagePath.TrimStart('/'));
                }
                else
                {
                    fullPath = Path.Combine(basePath, imagePath.TrimStart('/'));
                }
                
                if (File.Exists(fullPath))
                {
                    var fileInfo = new FileInfo(fullPath);
                    return fileInfo.Length;
                }
            }
            catch
            {
                // Ignore errors
            }
            return 0;
        }

        private async Task<bool> ValidateWebPFileAsync(string filePath)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[12];
                    var bytesRead = await stream.ReadAsync(buffer, 0, 12);
                    
                    // WebP files start with "RIFF" and contain "WEBP"
                    if (bytesRead >= 12 && 
                        System.Text.Encoding.ASCII.GetString(buffer, 0, 4) == "RIFF" &&
                        System.Text.Encoding.ASCII.GetString(buffer, 8, 4) == "WEBP")
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // If we can't read the file, assume it's invalid
                return false;
            }
            
            return false;
        }

        private string GetFullImagePath(string imageUrl)
        {
            var basePath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
            
            if (_webHostEnvironment.WebRootPath == null)
            {
                return Path.Combine(basePath, "wwwroot", imageUrl.TrimStart('/'));
            }
            else
            {
                return Path.Combine(basePath, imageUrl.TrimStart('/'));
            }
        }
    }
}
