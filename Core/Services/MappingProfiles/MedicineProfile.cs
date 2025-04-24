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
    public class MedicineProfile : Profile
    {
        public MedicineProfile()
        {
            CreateMap<Medicine, MedicineResponse>()
                .ForMember(d => d.StripPrice, opt => opt.MapFrom(s => s.StripPrice))
                .ForMember(d => d.BoxPrice, opt => opt.MapFrom(s => s.BoxPrice))
                .ForMember(d => d.Image, opt => opt.MapFrom<PictureUrlResolver>());
        }
    }
}
