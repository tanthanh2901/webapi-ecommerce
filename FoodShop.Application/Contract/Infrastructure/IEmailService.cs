using FoodShop.Application.Models.Mail;

namespace FoodShop.Application.Contract.Infrastructure
{
    public interface IEmailService
    {
        Task SendEmail(Email email);

    }
}
