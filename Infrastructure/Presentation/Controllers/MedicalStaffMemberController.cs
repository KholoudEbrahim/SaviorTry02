using Domain.Models.Enumerations;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalStaffMemberController : ControllerBase
    {
        private readonly IMedicalStaffMemberService _staffService;

        public MedicalStaffMemberController(IMedicalStaffMemberService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableStaff([FromQuery] MedicalRole role, [FromQuery] string day)
        {
            var staff = await _staffService.GetAvailableStaffAsync(role, day);
            return Ok(staff);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffMemberById(int id)
        {
            var staffMember = await _staffService.GetMedicalStaffMemberByIdAsync(id);
            if (staffMember == null)
            {
                return NotFound($"Staff member with ID {id} not found.");
            }
            return Ok(staffMember);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateStaffMember([FromBody] MedicalStaffMember staffMember)
        {
            var createdStaffMember = await _staffService.CreateMedicalStaffMemberAsync(staffMember);
            return CreatedAtAction(nameof(GetStaffMemberById), new { id = createdStaffMember.Id }, createdStaffMember);
        }
    
         [HttpGet("all")]
        public async Task<IActionResult> GetAllMedicalStaff()
        {


            try
            {
                var staffMembers = await _staffService.GetAllMedicalStaffAsync();
                if (!staffMembers.Any())
                {
                    return NotFound("No medical staff members found.");
                }
                return Ok(staffMembers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}