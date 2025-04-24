using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyController : ControllerBase
    {
        private readonly IEmergencyService _emergencyService;

        public EmergencyController(IEmergencyService emergencyService)
        {
            _emergencyService = emergencyService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> CreateEmergency([FromBody] Emergency emergency)
        {
            var createdEmergency = await _emergencyService.CreateEmergencyAsync(emergency);
            return CreatedAtAction(nameof(GetEmergencyById), new { id = createdEmergency.Id }, createdEmergency);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmergencyById(int id)
        {
            var emergency = await _emergencyService.GetEmergencyByIdAsync(id);
            return Ok(emergency);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetEmergenciesByUserId(int userId)
        {
            var emergencies = await _emergencyService.GetEmergenciesByUserIdAsync(userId);
            return Ok(emergencies);
        }
    }
}
