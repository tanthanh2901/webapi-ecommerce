﻿using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Products.Queries.GetAllProducts
{
    public class GetAllProductQuery : IRequest<List<ProductListDto>>
    {
    }
}
