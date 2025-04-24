using Domain.Models.Enumerations;
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
            return Ok(staffMember);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStaffMember([FromBody] MedicalStaffMember staffMember)
        {
            var createdStaffMember = await _staffService.CreateMedicalStaffMemberAsync(staffMember);
            return CreatedAtAction(nameof(GetStaffMemberById), new { id = createdStaffMember.Id }, createdStaffMember);
        }
    }
}
