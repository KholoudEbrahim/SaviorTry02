using Domain.Models;
using Shared.MedicalStaffDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface IMedicalRequestService
    {
        Task<MedicalRequest> CreateMedicalRequestAsync(MedicalRequest request);
        Task<IEnumerable<MedicalRequest>> GetAllRequestsAsync();
        Task<IEnumerable<MedicalRequest>> GetRequestsByUserIdAsync(int userId);
        Task<IEnumerable<MedicalRequest>> GetRequestsByStaffIdAsync(int staffId);
        Task<MedicalRequest?> GetMedicalRequestByIdAsync(int id);
     
    }
}
