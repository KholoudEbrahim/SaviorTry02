using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Shared.MedicineDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    internal class PictureUrlResolver(IConfiguration configuration) : IValueResolver<Medicine, MedicineResponse, string>
    {
        public string Resolve(Medicine source, MedicineResponse destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source.Image)) return string.Empty;

            // Concat Base URL with the relative path
            return $"{configuration["BaseUrl"]}{source.Image}";
        }
    }
}
