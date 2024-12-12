namespace FoodShop.Application.Contract.Infrastructure
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(Stream fileStream);
    }
}
