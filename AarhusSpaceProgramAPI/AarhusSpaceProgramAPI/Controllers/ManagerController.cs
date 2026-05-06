using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using AarhusSpaceProgramAPI.Models;

namespace AarhusSpaceProgramAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ManagerController : ControllerBase
{
    private readonly  ApplicationDbContext _context;
    private readonly ILogger<ManagerController> _logger;

    public ManagerController(ApplicationDbContext context, ILogger<ManagerController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    private void LogHttpCall(int statusCode)
    {
        _logger.LogInformation("HTTP call {@LogInfo}", new
        {
            HttpMethod = HttpContext.Request.Method,
            RequestPath = HttpContext.Request.Path.ToString(),
            StatusCode = statusCode,
            Timestamp = DateTimeOffset.UtcNow
        });
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManagerDto>>> GetManagers()
    {
        var manager = await _context.Managers
            .Select(m => new ManagerDto
            {
                ManagerId = m.ManagerId,
                Name = m.Name,
                Department =  m.Department,
                HireDate = m.HireDate,
            }).ToListAsync();
        
        return Ok(manager);
    }

    [HttpPost]
    public async Task<ActionResult<ManagerDto>> CreateManager([FromForm] ManagerDto dto)
    {
        var manager = new Manager
        {
            Name = dto.Name,
            Department = dto.Department,
            HireDate = dto.HireDate,
        };
        
        _context.Managers.Add(manager);
        await _context.SaveChangesAsync();

        var resultDto = new ManagerDto
        {
            ManagerId = manager.ManagerId,
            Name = manager.Name,
            Department = manager.Department,
            HireDate = manager.HireDate,
        };

        LogHttpCall(200);
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManager(int id, [FromForm] ManagerDto dto)
    {
        var  manager = await _context.Managers.FindAsync(id);
        if (manager == null)
        {
            LogHttpCall(404);
            return NotFound();
        }
        manager.Name = dto.Name;
        manager.Department = dto.Department;
        manager.HireDate = dto.HireDate;
        
        await _context.SaveChangesAsync();

        LogHttpCall(204);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        var manager = await _context.Managers.FindAsync(id);
        if (manager == null)
        {
            LogHttpCall(404);
            return NotFound();
        }

        _context.Managers.Remove(manager);
        await _context.SaveChangesAsync();

        LogHttpCall(204);
        return NoContent();
    }
    
}