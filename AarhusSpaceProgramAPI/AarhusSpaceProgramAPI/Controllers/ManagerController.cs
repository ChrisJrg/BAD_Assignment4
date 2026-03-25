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

    public ManagerController(ApplicationDbContext context)
    {
        _context = context;
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
    public async Task<ActionResult<ManagerDto>> CreateManager(ManagerDto dto)
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
        
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManager(int id, ManagerDto dto)
    {
        var  manager = await _context.Managers.FindAsync(id);
        if (manager == null)
        {
            return NotFound();
        }
        manager.Name = dto.Name;
        manager.Department = dto.Department;
        manager.HireDate = dto.HireDate;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        var manager = await _context.Managers.FindAsync(id);
        if (manager == null)
        {
            return NotFound();
        }

        _context.Managers.Remove(manager);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}