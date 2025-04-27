using Domain.Contracts;
using Domain.Models;
using ServicesAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmergencyService : IEmergencyService
    {
        private readonly IGenericRepository<Emergency> _emergencyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmergencyService(IGenericRepository<Emergency> emergencyRepository, IUnitOfWork unitOfWork)
        {
            _emergencyRepository = emergencyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Emergency> CreateEmergencyAsync(Emergency emergency)
        {
      
            emergency.Id = 0;
            Random random = new Random();
            int minutes = random.Next(10, 36);
            emergency.Duration = TimeSpan.FromMinutes(minutes);
            emergency.CreatedAt = DateTime.UtcNow;
            emergency.IsDeleted = false;
            await _emergencyRepository.AddAsync(emergency);
            await _unitOfWork.CompleteAsync();

            return emergency;
        }

        public async Task<Emergency> GetEmergencyByIdAsync(int id)
        {
            return await _emergencyRepository.GetByIdAsync(id) ?? throw new Exception("Emergency not found");
        }

        public async Task<IEnumerable<Emergency>> GetAllEmergenciesAsync()
        {
            return await _emergencyRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Emergency>> GetEmergenciesByUserIdAsync(int userId)
        {
            return await _emergencyRepository.FindAsync(e => e.UserID == userId);
        }
    }
}