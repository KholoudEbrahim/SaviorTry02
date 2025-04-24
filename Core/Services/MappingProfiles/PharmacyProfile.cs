using AutoMapper;
using Domain.Models;
using Shared.MedicineDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class PharmacyProfile : Profile

    {
        public PharmacyProfile()
        {

            CreateMap<Pharmacy, PharmacyResponse>()
                 .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src =>
                $"{src.Street}, {src.BuildingNumber}, {src.City}"))
            .ForMember(dest => dest.DistanceInKm, opt => opt.Ignore());

        }
    }
}
