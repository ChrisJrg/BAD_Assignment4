using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using  AarhusSpaceProgramAPI.Models;

namespace AarhusSpaceProgramAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AstronautController : ControllerBase
{
    private readonly  ApplicationDbContext _context;

    public AstronautController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<AstronautDto>>> GetAstronauts()
    {
        var astronauts = await _context.Astronauts
            .Select(a => new AstronautDto
            {
                AstronautId =  a.AstronautId,
                Name = a.Name,
                HireDate = a.HireDate,
                PayGrade = a.PayGrade,
                Rank = a.Rank,
                EXPInSim = a.EXPInSim,
                EXPInSpace = a.EXPInSpace,
            }).ToListAsync();
        
        return Ok(astronauts);
    }

    [HttpPost]
    public async Task<ActionResult<AstronautDto>> CreateAstronaut(AstronautDto dto)
    {
        var astronaut = new Astronaut
        {
            Name = dto.Name,
            HireDate =  dto.HireDate,
            PayGrade = dto.PayGrade,
            Rank = dto.Rank,
            EXPInSim = dto.EXPInSim,
            EXPInSpace = dto.EXPInSpace,
        };
        
        _context.Astronauts.Add(astronaut);
        await _context.SaveChangesAsync();

        var resultDto = new AstronautDto
        {
            AstronautId = astronaut.AstronautId,
            Name = astronaut.Name,
            HireDate = astronaut.HireDate,
            PayGrade = astronaut.PayGrade,
            Rank = astronaut.Rank,
            EXPInSim = astronaut.EXPInSim,
            EXPInSpace = astronaut.EXPInSpace,
        };
        
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAstronaut(int id, AstronautDto dto)
    {
        var  astronaut = await _context.Astronauts.FindAsync(id);
        if (astronaut == null)
        {
            return NotFound();
        }
        astronaut.Name = dto.Name;
        astronaut.HireDate = dto.HireDate;
        astronaut.PayGrade = dto.PayGrade;
        astronaut.Rank = dto.Rank;
        astronaut.EXPInSim = dto.EXPInSim;
        astronaut.EXPInSpace = dto.EXPInSpace;
        
        _context.Entry(astronaut).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAstronaut(int id)
    {
        var astronaut = await _context.Astronauts.FindAsync(id);
        if (astronaut == null)
        {
            return NotFound();
        }

        _context.Astronauts.Remove(astronaut);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}