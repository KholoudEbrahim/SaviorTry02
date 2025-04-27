using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface IEmergencyService
    {
        Task<Emergency> CreateEmergencyAsync(Emergency emergency);
        Task<Emergency> GetEmergencyByIdAsync(int id);
        Task<IEnumerable<Emergency>> GetAllEmergenciesAsync();
        Task<IEnumerable<Emergency>> GetEmergenciesByUserIdAsync(int userId);

    }
}
