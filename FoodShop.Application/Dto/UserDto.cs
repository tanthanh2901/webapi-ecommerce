using FoodShop.Domain.Entities;

namespace FoodShop.Application.Dto
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string PhoneNumber { get; set; }


    }
}
