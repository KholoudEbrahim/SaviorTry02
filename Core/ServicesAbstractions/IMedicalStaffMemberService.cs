using Domain.Models.Enumerations;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface IMedicalStaffMemberService
    {
        Task<MedicalStaffMember> CreateMedicalStaffMemberAsync(MedicalStaffMember staffMember);
        Task<MedicalStaffMember> GetMedicalStaffMemberByIdAsync(int id);
        Task<IEnumerable<MedicalStaffMember>> GetAvailableStaffAsync(MedicalRole role, string day);
        Task<IEnumerable<MedicalStaffMember>> GetAllMedicalStaffAsync();
    }
}
