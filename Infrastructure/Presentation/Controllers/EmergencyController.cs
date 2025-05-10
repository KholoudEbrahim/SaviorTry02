using Domain.Models;
using Domain.Models.Enumerations;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using Shared.EmergencyDTOs;
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
        public async Task<IActionResult> CreateEmergency([FromBody] EmergencyRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid emergency data.");
            }
            var emergency = new Emergency
            {
                UserID = request.UserID,
                Location = request.Location,
                Type = request.Type,
                IsConfirmed = request.IsConfirmed
            };
          
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
        [HttpGet("all")]
        public async Task<IActionResult> GetAllEmergencies([FromQuery] DateTime? fromDate, [FromQuery] int? type)
        {
            var emergencies = await _emergencyService.GetAllEmergenciesAsync();

            if (fromDate.HasValue)
            {
                emergencies = emergencies.Where(e => e.CreatedAt >= fromDate.Value);
            }
            if (type.HasValue)
            {

                emergencies = emergencies.Where(e => e.Type == (EmergencyType)type.Value);
            }
            return Ok(emergencies);
        }
    }

}
