using Microsoft.AspNetCore.Mvc;
using ApbdTest.Services;
using ApbdTest.Exceptions;
using ApbdTest.DTOs;

namespace ApbdTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitsController : ControllerBase
{
        private readonly IDbService _dbService;

        public VisitsController(IDbService dbService)
        {
                _dbService = dbService;
        }
        
        //Returns the information about the visit
        [HttpGet("{visitId}")]
        public async Task<IActionResult> GetVisit(int visitId)
        {
                try
                {
                        var result = await _dbService.GetVisitByIdAsync(visitId);
                        return Ok(result);
                }
                catch (NotFoundException e)
                {
                        return NotFound(e.Message);
                }
        }
        
        //Adds a visit if all the request data passes the validation
        [HttpPost]
        public async Task<IActionResult> AddVisit(AddVisitDTO data)
        {

                if (!ModelState.IsValid)
                {
                        return BadRequest(ModelState);
                }

                try
                {
                        var result = await _dbService.AddNewVisitAsync(data);
                        return Ok(result);
                }
                catch (NotFoundException e)
                {
                        return NotFound(e.Message);
                }
                catch (Exception ex)
                {
                        return BadRequest(ex.Message);
                }
            
        }
         
        
}