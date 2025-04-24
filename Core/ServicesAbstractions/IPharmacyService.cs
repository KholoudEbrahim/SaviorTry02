using Shared.MedicineDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IPharmacyService
    {
        Task<IEnumerable<PharmacyResponse>> GetAllPharmaciesAsync();
        Task<List<PharmacyResponse>> GetNearestPharmacies(double userLat, double userLon);
        Task<PharmacyDetailsResponse> GetPharmacyDetailsAsync(int pharmacyId);
        //Task<CartDTO> AddToCart(int medicineId, int quantity, int pharmacyId);
        //Task<CartDTO> GetCart();
        //Task ClearCart();
    }
}
