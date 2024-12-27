namespace FoodShop.Application.Feature.User.Model
{
    public class UpdateUserInfoViewModel
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string ShipName { get; set; } = string.Empty;
        public string ShipAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
    }
}
