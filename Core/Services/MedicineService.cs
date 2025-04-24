using AutoMapper;
using Domain.Contracts;
using ServicesAbstractions;
using Shared.MedicineDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MedicineService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Get All Medicines
        public async Task<IEnumerable<MedicineResponse>> GetAllMedicinesAsync()
        {
            var medicines = await _unitOfWork.Medicines.GetAllAsync();
            return _mapper.Map<List<MedicineResponse>>(medicines);
        }

        // Get Medicine Details by ID
        public async Task<MedicineResponse> GetMedicineDetailsAsync(int medicineId)
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(medicineId);
            if (medicine == null)
            {
                throw new KeyNotFoundException("Medicine not found");
            }
            return _mapper.Map<MedicineResponse>(medicine);
        }

        // Search Medicines (Case Insensitive)
        public async Task<IEnumerable<MedicineResponse>> SearchMedicinesAsync(string searchTerm)
        {
            // Use ToLower() for case-insensitive search
            var medicines = await _unitOfWork.Medicines
                .FindAsync(m => m.Name.ToLower().Contains(searchTerm.ToLower()) ||
                                m.Description.ToLower().Contains(searchTerm.ToLower()));

            return _mapper.Map<List<MedicineResponse>>(medicines);
        }

        //// Add Medicine to Cart
        //public async Task AddToCartAsync(int medicineId, int quantity)
        //{
        //    var medicine = await _unitOfWork.Medicines.GetByIdAsync(medicineId);
        //    if (medicine == null)
        //    {
        //        throw new KeyNotFoundException("Medicine not found");
        //    }

        //    // Logic to add to cart (You can use a Cart entity or session-based cart)
        //    // For now, we'll just log the action
        //    Console.WriteLine($"Added {quantity} of {medicine.Name} to cart.");
        //}
    }
}
