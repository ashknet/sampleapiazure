using EntraApi.Data;
using EntraApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntraApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class Home2Controller : ControllerBase
{
  private readonly AppDbContext _dbContext;

  public Home2Controller(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  [HttpGet("employees-ef")] 
  public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesEf()
  {
    var employees = await _dbContext.Employees.AsNoTracking().ToListAsync();
    return Ok(employees);
  }
}

