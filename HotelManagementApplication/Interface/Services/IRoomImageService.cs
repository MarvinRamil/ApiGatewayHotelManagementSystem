using HotelManagementApplication.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Services
{
    public interface IRoomImageService
    {
        Task<ICollection<RoomImageResponseDto>> GetRoomImagesAsync(int roomId);
        Task<RoomImageResponseDto?> GetRoomImageByIdAsync(int imageId);
        Task<ICollection<RoomImageResponseDto>> UploadRoomImagesAsync(RoomImageUploadDto uploadDto);
        Task<bool> DeleteRoomImageAsync(int imageId);
        Task<bool> DeleteRoomImageByUrlAsync(string imageUrl);
        Task<bool> UpdateRoomImagesAsync(RoomImageUpdateDto updateDto);
        Task<bool> SetPrimaryImageAsync(int roomId, string imageUrl);
        Task<bool> ConvertImageToWebPAsync(string imagePath);
        Task<string> GenerateUniqueFileNameAsync(string originalFileName);
        Task<bool> ValidateImageFileAsync(IFormFile file);
        Task<long> GetImageFileSizeAsync(string imagePath);
    }
}
