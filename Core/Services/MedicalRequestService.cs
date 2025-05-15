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
    public class MedicalRequestService : IMedicalRequestService
    {
        private readonly IGenericRepository<MedicalRequest> _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MedicalRequestService(IGenericRepository<MedicalRequest> requestRepository, IUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MedicalRequest> CreateMedicalRequestAsync(MedicalRequest request)
        {
            await _requestRepository.AddAsync(request);
            await _unitOfWork.CompleteAsync();
            return request;
        }

        public async Task<IEnumerable<MedicalRequest>> GetAllRequestsAsync()
        {
            return await _requestRepository.GetAllAsync();
        }

        public async Task<IEnumerable<MedicalRequest>> GetRequestsByUserIdAsync(int userId)
        {
            return await _requestRepository.FindAsync(r => r.UserId == userId);
        }

        public async Task<IEnumerable<MedicalRequest>> GetRequestsByStaffIdAsync(int staffId)
        {
            return await _requestRepository.FindAsync(r => r.MedicalStaffMemberId == staffId);
        }
        public async Task<MedicalRequest?> GetMedicalRequestByIdAsync(int id)
        {
            return await _requestRepository.GetByIdAsync(id);
        }

    }
}
