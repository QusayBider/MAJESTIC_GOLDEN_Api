using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class FileService : IFileService
    {
        private string _FilePath;
        public FileService()
        {
            _FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files");
            if (!Directory.Exists(_FilePath))
            {
                Directory.CreateDirectory(_FilePath);
            }
        }
        public void DeleteFileAsync(string fileName)
        {
            var filepath = Path.Combine(_FilePath, fileName);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }
            else
            {
                var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_FilePath, FileName);
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                return FileName;
            }


        }
        public async Task<List<string>> UploadManyFileAsync(List<IFormFile> files)
        {

            var FileNames = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is null or empty", nameof(file));
                }
                else
                {
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_FilePath, FileName);
                    using (var stream = File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                    FileNames.Add(FileName);
                }
            }

            return FileNames;
        }
    }
}
