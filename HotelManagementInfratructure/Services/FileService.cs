using HotelManagementApplication.Interface.Services;
using Microsoft.AspNetCore.Hosting;

namespace HotelManagementInfrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(FileUploadRequest file, string folder)
        {
            var uploadPath = Path.Combine(_environment.WebRootPath, folder);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.Content.CopyToAsync(stream);
            }

            return $"/{folder}/{fileName}";
        }

        public void DeleteFile(string relativePath)
        {
            var filePath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
