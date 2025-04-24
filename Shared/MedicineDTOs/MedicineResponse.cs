using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MedicineDTOs
{

    public record MedicineResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? StripPrice { get; set; }
        public decimal? BoxPrice { get; set; }
        public string Image { get; set; } = string.Empty;
    }

}
