using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Shared.MedicineDTOs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{

   
        public class PharmacyService : IPharmacyService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
        //  private readonly ICacheService _cacheService;


        public PharmacyService(IUnitOfWork unitOfWork, IMapper mapper) //ICacheService cacheService
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //    _cacheService = cacheService;
        //}
         public async Task<IEnumerable<PharmacyResponse>> GetAllPharmaciesAsync()
        {
            var pharmacies = await _unitOfWork.Pharmacies.GetAllAsync();
            return _mapper.Map<List<PharmacyResponse>>(pharmacies);
        }
      
        public async Task<List<PharmacyResponse>> GetNearestPharmacies(double userLat, double userLon)
            {
                var pharmacies = await _unitOfWork.Pharmacies.GetAllAsync();

                return pharmacies
                    .Select(p => new PharmacyResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        FullAddress = $"{p.Street}, {p.City}",
                        HasDelivery = p.HasDelivery,
                        DistanceInKm = CalculateDistance(userLat, userLon, p.Latitude, p.Longitude)
                    })
                    .OrderBy(p => p.DistanceInKm)
                    .ToList();
            }


        public async Task<PharmacyDetailsResponse> GetPharmacyDetailsAsync(int pharmacyId)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(pharmacyId);
            if (pharmacy == null)
            {
                throw new KeyNotFoundException("Pharmacy not found");
            }

            var availableMedicines = await _unitOfWork.Supplies
                .FindAsync(s => s.PharmacyId == pharmacyId );

            var medicineResponses = availableMedicines.Select(s => new MedicineResponse
            {
                Id = s.Medicine.Id,
                Name = s.Medicine.Name,
                Description = s.Medicine.Description,
                StripPrice = s.Medicine.StripPrice,
                BoxPrice = s.Medicine.BoxPrice,
                Image = s.Medicine.Image
            }).ToList();

            return new PharmacyDetailsResponse
            {
                Id = pharmacy.Id,
                Name = pharmacy.Name,
                FullAddress = $"{pharmacy.Street}, {pharmacy.City}",
                HasDelivery = pharmacy.HasDelivery,
                AvailableMedicines = medicineResponses
            };
        }
        //public async Task<CartDTO> AddToCart(int medicineId, int quantity, int pharmacyId)
        //{
        //    var medicine = await _unitOfWork.Medicines.GetByIdAsync(medicineId);
        //    var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(pharmacyId);

        //    var supply = await _unitOfWork.Supplies.FindAsync(s =>
        //        s.MedicineId == medicineId && s.PharmacyId == pharmacyId);

        //    if (supply == null || supply.StockQuantity < quantity)
        //    {
        //        throw new Exception("Not enough stock available");
        //    }

        //    var cart = await GetCart();
        //    var existingItem = cart.Items.FirstOrDefault(i => i.MedicineId == medicineId);

        //    if (existingItem != null)
        //    {
        //        existingItem.Quantity += quantity;
        //    }
        //    else
        //    {
        //        cart.Items.Add(new CartItemDTO
        //        {
        //            MedicineId = medicine.Id,
        //            MedicineName = medicine.Name,
        //            Price = supply.Price,
        //            Quantity = quantity,
        //            ImageUrl = medicine.ImageUrl
        //        });
        //    }

        //    cart.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
        //    cart.TotalItems = cart.Items.Sum(i => i.Quantity);

        //    await _cacheService.SetAsync("user_cart", cart);
        //    return cart;
        //}

        //public async Task<CartDTO> GetCart()
        //{
        //    var cart = await _cacheService.GetAsync<CartDTO>("user_cart");
        //    return cart ?? new CartDTO();
        //}

        //public async Task ClearCart()
        //{
        //    await _cacheService.RemoveAsync("user_cart");
        //}

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
            {
             
                var R = 6371;                  // Earth radius in km
                var dLat = ToRadians(lat2 - lat1);
                var dLon = ToRadians(lon2 - lon1);
                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
      
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                return R * c;
            }

            private double ToRadians(double angle) => angle * (Math.PI / 180);

       
    }
    
}
