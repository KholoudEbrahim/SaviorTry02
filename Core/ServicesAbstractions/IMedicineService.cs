using Shared.MedicineDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface IMedicineService
    {
        Task<IEnumerable<MedicineResponse>> GetAllMedicinesAsync();
        Task<MedicineResponse> GetMedicineDetailsAsync(int medicineId);
        Task<IEnumerable<MedicineResponse>> SearchMedicinesAsync(string searchTerm);
  
    }
}
        //Task AddToCartAsync(int medicineId, int quantity);
