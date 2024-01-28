using belajarnetapi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace belajarnetapi.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("employees")]
        public IActionResult GetEmployees()
        {
            var response = new ApiResponse<string>();
            response.Success = true;
            response.Data = "Get Employees";
            return Ok(response);
        }      
    }
}