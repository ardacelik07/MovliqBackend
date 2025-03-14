namespace RunningApplicationNew.Services
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;

    public interface IPhotoService
    {
        Task<string> UploadProfilePhotoAsync(IFormFile file, string userId);
    }

    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadProfilePhotoAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya yok veya boş");

            using var stream = file.OpenReadStream();

            // Profil resimleri için yükleme parametreleri
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "profile_photos",
                PublicId = $"user_{userId}_{Guid.NewGuid()}",
                Transformation = new Transformation()
                    .Width(500)  // Max genişlik
                    .Height(500) // Max yükseklik
                    .Crop("fill") // Otomatik kırpma
                    .Quality(80)  // Kalite (1-100)
            };

            // Yükleme işlemi
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary yükleme hatası: {uploadResult.Error.Message}");

            // Yüklenen resmin URL'ini döndür
            return uploadResult.SecureUrl.ToString();
        }
    }
}
