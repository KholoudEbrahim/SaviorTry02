using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MedicineDTOs
{
    public record PharmacyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public bool HasDelivery { get; set; }
        public double DistanceInKm { get; set; }
    }
}
