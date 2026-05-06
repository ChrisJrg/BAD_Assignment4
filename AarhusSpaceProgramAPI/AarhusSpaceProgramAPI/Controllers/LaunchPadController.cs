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
    private readonly ILogger<LaunchPadController> _logger;

    public LaunchPadController(ApplicationDbContext context,  ILogger<LaunchPadController> logger)
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
    public async Task<ActionResult<LaunchPadDto>> CreateLaunchPad([FromBody] LaunchPadDto dto)
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

        LogHttpCall(200);
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLaunchPad(int id, [FromBody] LaunchPadDto dto)
    {
        var  launchPad = await _context.LaunchPads.FindAsync(id);
        if (launchPad == null)
        {
            LogHttpCall(404);
            return NotFound();
        }
        launchPad.Location = dto.Location;
        launchPad.MaxWeight = dto.MaxWeight;
        launchPad.CurrentStatus = dto.CurrentStatus;
        
        await _context.SaveChangesAsync();
        LogHttpCall(204);
        
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
        
        LogHttpCall(204);
        return NoContent();
    }
    
}