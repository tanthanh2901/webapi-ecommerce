using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Dto
{
    public record TokenDto(string accessToken, string refreshToken)
    {
    }
}
