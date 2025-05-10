using AutoMapper;
using Domain.Models.CartEntities;
using Microsoft.Extensions.Configuration;
using Shared.CartDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    internal class CartItemImageUrlResolver(IConfiguration configuration) : IValueResolver<CartItem, CartItemResponse, string>
    {
        public string Resolve(CartItem source, CartItemResponse destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source.Medicine?.Image)) return string.Empty;

            return $"{configuration["BaseUrl"]}{source.Medicine.Image}";
        }
    }

}
