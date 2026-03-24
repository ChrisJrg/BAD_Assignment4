using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using  AarhusSpaceProgramAPI.Models;

namespace AarhusSpaceProgramAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class LaunchPadController : ControllerBase
{
    private readonly  ApplicationDbContext _context;

    public LaunchPadController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<LaunchPadDto>>> GetLaunchPads()
    {
        var launchPad = await _context.LaunchPads
            .Select(l => new LaunchPadDto
            {
                LaunchPadId =  l.LaunchPadId,
                Location = l.Location,
                MaxWeight =  l.MaxWeight,
                CurrentStatus =  l.CurrentStatus,
            }).ToListAsync();
        
        return Ok(launchPad);
    }

    [HttpPost]
    public async Task<ActionResult<LaunchPadDto>> CreateLaunchPad(LaunchPadDto dto)
    {
        var launchPad = new LaunchPad
        {
            Location =  dto.Location,
            MaxWeight = dto.MaxWeight,
            CurrentStatus = dto.CurrentStatus,
        };
        
        _context.LaunchPads.Add(launchPad);
        await _context.SaveChangesAsync();

        var resultDto = new LaunchPadDto
        {
            LaunchPadId = launchPad.LaunchPadId,
            Location = launchPad.Location,
            MaxWeight = launchPad.MaxWeight,
            CurrentStatus = launchPad.CurrentStatus,
        };
        
        return CreatedAtAction(nameof(GetLaunchPads), new { id = launchPad.LaunchPadId}, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLaunchPad(int id, LaunchPadDto dto)
    {
        var  launchPad = await _context.LaunchPads.FindAsync(id);
        if (launchPad == null)
        {
            return NotFound();
        }
        launchPad.LaunchPadId = dto.LaunchPadId;
        launchPad.Location = dto.Location;
        launchPad.MaxWeight = dto.MaxWeight;
        launchPad.CurrentStatus = dto.CurrentStatus;
        
        _context.Entry(launchPad).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLaunchPad(int id)
    {
        var launchPad = await _context.LaunchPads.FindAsync(id);
        if (launchPad == null)
        {
            return NotFound();
        }

        _context.LaunchPads.Remove(launchPad);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}