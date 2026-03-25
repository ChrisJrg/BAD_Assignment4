using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using  AarhusSpaceProgramAPI.Models;

namespace AarhusSpaceProgramAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ScientistController : ControllerBase
{
    private readonly  ApplicationDbContext _context;

    public ScientistController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScientistDto>>> GetScientists()
    {
        var scientists = await _context.Scientists
            .Select(s => new ScientistDto
            {
                ScientistId =  s.ScientistId,
                Name = s.Name,
                HireDate = s.HireDate,
                Title = s.Title,
                Specialty = s.Specialty,
            }).ToListAsync();
        
        return Ok(scientists);
    }

    [HttpPost]
    public async Task<ActionResult<ScientistDto>> CreateScientist([FromForm]ScientistDto dto)
    {
        var scientist = new Scientist
        {
            ScientistId =  dto.ScientistId,
            Name = dto.Name,
            HireDate = dto.HireDate,
            Title = dto.Title,
            Specialty = dto.Specialty,
        };
        
        _context.Scientists.Add(scientist);
        await _context.SaveChangesAsync();

        var resultDto = new ScientistDto
        {
            ScientistId =  scientist.ScientistId,
            Name = scientist.Name,
            HireDate = scientist.HireDate,
            Title = scientist.Title,
            Specialty = scientist.Specialty,
        };
        
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScientist(int id, [FromForm]ScientistDto dto)
    {
        var  scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null)
        {
            return NotFound();
        }

        scientist.Name = dto.Name;
        scientist.HireDate = dto.HireDate;
        scientist.Title = dto.Title;
        scientist.Specialty = dto.Specialty;
        
        _context.Entry(scientist).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScientist(int id)
    {
        var scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null)
        {
            return NotFound();
        }

        _context.Scientists.Remove(scientist);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}