using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Services
{
    public class FileUploadRequest
    {
        public string FileName { get; set; } = string.Empty;
        public Stream Content { get; set; } = Stream.Null;
    }

    public interface IFileService
    {
        Task<string> SaveFileAsync(FileUploadRequest file, string folder);
        void DeleteFile(string relativePath);
    }
}
