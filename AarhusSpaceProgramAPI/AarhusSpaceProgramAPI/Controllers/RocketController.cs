using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using  AarhusSpaceProgramAPI.Models;

namespace AarhusSpaceProgramAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class RocketController : ControllerBase
{
    private readonly  ApplicationDbContext _context;

    public RocketController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<RocketDto>>> GetRocket()
    {
        var rockets = await _context.Rockets
            .Select(r => new RocketDto
            {
                RocketId =  r.RocketId,
                Model = r.Model,
                Weight = r.Weight,
                CrewCapacity = r.CrewCapacity,
                Stages = r.Stages,
                FuelCapacity =  r.FuelCapacity,
                PayloadCapacity =  r.PayloadCapacity,
            }).ToListAsync();
        
        return Ok(rockets);
    }

    [HttpPost]
    public async Task<ActionResult<RocketDto>> CreateRocket([FromForm]RocketDto dto)
    {
        var rocket = new Rocket
        {
            RocketId =  dto.RocketId,
            Model = dto.Model,
            Weight = dto.Weight,
            CrewCapacity = dto.CrewCapacity,
            Stages = dto.Stages,
            FuelCapacity =  dto.FuelCapacity,
            PayloadCapacity =  dto.PayloadCapacity,
        };
        
        _context.Rockets.Add(rocket);
        await _context.SaveChangesAsync();

        var resultDto = new RocketDto
        {
            RocketId =  rocket.RocketId,
            Model = rocket.Model,
            Weight = rocket.Weight,
            CrewCapacity = rocket.CrewCapacity,
            Stages = rocket.Stages,
            FuelCapacity =  rocket.FuelCapacity,
            PayloadCapacity =  rocket.PayloadCapacity,
        };
        
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRocket(int id, [FromForm]RocketDto dto)
    {
        var  rocket = await _context.Rockets.FindAsync(id);
        if (rocket == null)
        {
            return NotFound();
        }

        rocket.RocketId =  dto.RocketId;
        rocket.Model = dto.Model;
        rocket.Weight = dto.Weight;
        rocket.CrewCapacity = dto.CrewCapacity;
        rocket.Stages = dto.Stages;
        rocket.FuelCapacity =  dto.FuelCapacity;
        rocket.PayloadCapacity = dto.PayloadCapacity;
        
        _context.Entry(rocket).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRocket(int id)
    {
        var rocket = await _context.Rockets.FindAsync(id);
        if (rocket == null)
        {
            return NotFound();
        }

        _context.Rockets.Remove(rocket);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}