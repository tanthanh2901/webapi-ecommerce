﻿using FoodShop.Domain.Enum;

namespace FoodShop.Application.Dto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderDetailDto> OrderDetailsDto { get; set; }
    }
}
