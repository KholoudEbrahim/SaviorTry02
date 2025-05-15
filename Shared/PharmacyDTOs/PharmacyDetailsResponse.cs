using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MedicineDTOs
{
    public record PharmacyDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullAddress { get; set; }
        public bool HasDelivery { get; set; }
        public string Phone { get; set; } = string.Empty;
        public List<MedicineResponse> AvailableMedicines { get; set; } = new List<MedicineResponse>();
    }
}
