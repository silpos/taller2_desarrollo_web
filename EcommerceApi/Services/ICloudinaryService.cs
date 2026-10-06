namespace EcommerceApi.Services
{
    public interface ICloudinaryService
    {
        // Recibe la imagen en Base64 y devuelve la URL segura generada por Cloudinary.
        Task<string> UploadImageAsync(string base64Image);
    }
}
