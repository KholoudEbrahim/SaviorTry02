using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        // GET: api/medicines
        [HttpGet]
        public async Task<IActionResult> GetAllMedicines()
        {
            var medicines = await _medicineService.GetAllMedicinesAsync();
            return Ok(medicines);
        }

        // GET: api/medicines/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicineDetails(int id)
        {
            try
            {
                var medicine = await _medicineService.GetMedicineDetailsAsync(id);
                return Ok(medicine);
            }
            catch (KeyNotFoundException ex)
            {
                // Return 404 Not Found if the medicine is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error for any other exceptions
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        // GET: api/medicines/search?term=
        [HttpGet("search")]
        public async Task<IActionResult> SearchMedicines([FromQuery] string term)
        {
            var medicines = await _medicineService.SearchMedicinesAsync(term);
            return Ok(medicines);
        }

        //// POST: api/medicines/add-to-cart
        //[HttpPost("add-to-cart")]
        //public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        //{
        //    await _medicineService.AddToCartAsync(request.MedicineId, request.Quantity);
        //    return Ok(new { Message = "Medicine added to cart successfully." });
        //}
    }
}
