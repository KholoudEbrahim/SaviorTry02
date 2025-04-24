using Microsoft.AspNetCore.Mvc;
using Services;
using Shared.MedicineDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmaciesController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;

        public PharmaciesController(IPharmacyService pharmacyService)
        {
            _pharmacyService = pharmacyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PharmacyResponse>>> GetPharmacies()
        {
            return Ok(await _pharmacyService.GetAllPharmaciesAsync());
        }

        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearestPharmacies([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var pharmacies = await _pharmacyService.GetNearestPharmacies(latitude, longitude);
            return Ok(pharmacies);
        }
        // GET: api/pharmacies/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacyDetails(int id)
        {
            var pharmacyDetails = await _pharmacyService.GetPharmacyDetailsAsync(id);
            return Ok(pharmacyDetails);
        }


    }
}
