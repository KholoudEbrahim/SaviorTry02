using Domain.Contracts;
using Domain.Models.Enumerations;
using Domain.Models;
using ServicesAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace Services
{
    public class MedicalStaffMemberService : IMedicalStaffMemberService
    {
        private readonly IGenericRepository<MedicalStaffMember> _staffRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MedicalStaffMemberService(IGenericRepository<MedicalStaffMember> staffRepository, IUnitOfWork unitOfWork)
        {
            _staffRepository = staffRepository;
            _unitOfWork = unitOfWork;
      

        }

        public async Task<MedicalStaffMember> CreateMedicalStaffMemberAsync(MedicalStaffMember staffMember)
        {
            await _staffRepository.AddAsync(staffMember);
            await _unitOfWork.CompleteAsync();
            return staffMember;
        }

        public async Task<MedicalStaffMember?> GetMedicalStaffMemberByIdAsync(int id)
        {
            var query = _staffRepository.GetQueryable();
            return await EntityFrameworkQueryableExtensions
                .Include(query, m => m.Availability)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<IEnumerable<MedicalStaffMember>> GetAvailableStaffAsync(MedicalRole role, string day)
        {
            return await _staffRepository.FindAsync(s =>
                s.Role == role &&
                s.Availability.Any(a => a.Day == day));
        }
       
    }
    
}
