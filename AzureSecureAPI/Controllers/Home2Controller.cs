using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AzureSecureAPI.Data;
using AzureSecureAPI.Models;

namespace AzureSecureAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Home2Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Home2Controller> _logger;

        public Home2Controller(ApplicationDbContext context, ILogger<Home2Controller> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Get employees using Entity Framework
        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees using Entity Framework");
                return StatusCode(500, "An error occurred while fetching employees");
            }
        }

        // Get employee by ID
        [HttpGet("employees/{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return NotFound($"Employee with ID {id} not found");
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee by ID");
                return StatusCode(500, "An error occurred while fetching the employee");
            }
        }

        // Create new employee
        [HttpPost("employees")]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(500, "An error occurred while creating the employee");
            }
        }

        // Update employee
        [HttpPut("employees/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest("Employee ID mismatch");
            }

            try
            {
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EmployeeExists(id))
                {
                    return NotFound($"Employee with ID {id} not found");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee");
                return StatusCode(500, "An error occurred while updating the employee");
            }
        }

        // Delete employee
        [HttpDelete("employees/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound($"Employee with ID {id} not found");
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee");
                return StatusCode(500, "An error occurred while deleting the employee");
            }
        }

        private async Task<bool> EmployeeExists(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }
    }
}