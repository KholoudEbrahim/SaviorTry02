using AutoMapper;
using Domain.Models.CartEntities;
using Shared.CartDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartResponse>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CartItem, CartItemResponse>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.Name))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Price));


        }
    }
}
