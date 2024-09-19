namespace FoodShop.Application.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public CartDto Cart { get; set; }
        public int CartId { get; set; }
        public string Username { get; set; }

    }
}
