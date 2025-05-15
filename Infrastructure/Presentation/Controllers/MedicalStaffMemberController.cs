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
using Shared.MedicalStaffDTOs;
using AutoMapper;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalStaffMemberController : ControllerBase
    {
        private readonly IMedicalStaffMemberService _staffService;
        private readonly IMedicalRequestService _requestService;
        private readonly IMapper _mapper;

        public MedicalStaffMemberController(IMedicalStaffMemberService staffService, IMedicalRequestService requestService, IMapper mapper)
        {
            _staffService = staffService;
            _requestService = requestService;
            _mapper = mapper;

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
            var dto = new MedicalStaffMemberDto
            {
                Id = staffMember.Id,
                Name = staffMember.Name,
                Role = staffMember.Role,
                Phone = staffMember.Phone,
                Price = staffMember.Price,
                Location = staffMember.Location,
                Availability = staffMember.Availability.Select(a => new AvailabilityEntryDto
                {
                    Day = a.Day,
                    FromTime = a.FromTime,
                    ToTime = a.ToTime
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("request/{id}")]
        public async Task<IActionResult> GetMedicalRequestById(int id)
        {
            var request = await _requestService.GetMedicalRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound($"Medical request with ID {id} not found.");
            }
            return Ok(request);
        }


        [HttpPost("request")]
        public async Task<IActionResult> CreateRequest([FromBody] MedicalRequestDto requestDto)
        {
            if (requestDto == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
            
                var request = _mapper.Map<MedicalRequest>(requestDto);

                var createdRequest = await _requestService.CreateMedicalRequestAsync(request);

                return CreatedAtAction(nameof(GetMedicalRequestById), new { id = createdRequest.Id }, createdRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("user/{userId}/requests")]
        public async Task<IActionResult> GetRequestsByUser(int userId)
        {
            var requests = await _requestService.GetRequestsByUserIdAsync(userId);
            return Ok(requests);
        }



        [HttpGet("requests/all")]
        public async Task<IActionResult> GetAllMedicalRequests()
        {
            try
            {
                var allRequests = await _requestService.GetAllRequestsAsync();
                return Ok(allRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

    }
}