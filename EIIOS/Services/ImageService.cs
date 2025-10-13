using EIIOS.Data;
using EIIOS.Models;
using Microsoft.EntityFrameworkCore;

namespace EIIOS.Services
{
    public class ImageService
    {
        private readonly EIIOSDbContext _context;

        public ImageService(EIIOSDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Converts IFormFile to Base64 and saves as ImageModel
        /// </summary>
        public async Task<ImageModel?> SaveImageAsync(IFormFile file, string? altText = null)
        {
            if (file == null || file.Length == 0)
                return null;

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return null;

            // Validate file size (e.g., max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return null;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(fileBytes);

            var image = new ImageModel
            {
                FileName = file.FileName,
                Base64Data = base64String,
                ContentType = file.ContentType,
                FileSize = file.Length,
                AltText = altText,
                CreatedAt = DateTime.UtcNow
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return image;
        }

        /// <summary>
        /// Deletes an image by ID
        /// </summary>
        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var image = await _context.Images.FindAsync(imageId);
            if (image == null)
                return false;

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets image by ID
        /// </summary>
        public async Task<ImageModel?> GetImageAsync(int imageId)
        {
            return await _context.Images.FindAsync(imageId);
        }
    }
}