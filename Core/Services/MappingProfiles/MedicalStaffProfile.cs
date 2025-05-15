using AutoMapper;
using Domain.Models;
using Shared.MedicalStaffDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class MedicalStaffProfile : Profile
    {
        public MedicalStaffProfile()
        {
            CreateMap<MedicalRequestDto, MedicalRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.MedicalStaffMember, opt => opt.Ignore());

           
            CreateMap<MedicalRequest, MedicalRequestDto>();
        }
    }
}
