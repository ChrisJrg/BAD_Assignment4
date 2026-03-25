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
    public async Task<ActionResult<LaunchPadDto>> CreateLaunchPad([FromForm] LaunchPadDto dto)
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
        
        _logger.LogInformation("HTTP call {@LogInfo}", new
        {
            HttpMethod = HttpContext.Request.Method,
            RequestPath = HttpContext.Request.Path.ToString(),
            StatusCode = 200,
            Timestamp = DateTimeOffset.UtcNow
        });
        
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLaunchPad(int id, [FromForm] LaunchPadDto dto)
    {
        var  launchPad = await _context.LaunchPads.FindAsync(id);
        if (launchPad == null)
        {
            _logger.LogInformation("HTTP call {@LogInfo}", new
            {
                HttpMethod = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path.ToString(),
                StatusCode = 404,
                Timestamp = DateTimeOffset.UtcNow
            });
            return NotFound();
        }
        launchPad.LaunchPadId = dto.LaunchPadId;
        launchPad.Location = dto.Location;
        launchPad.MaxWeight = dto.MaxWeight;
        launchPad.CurrentStatus = dto.CurrentStatus;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("HTTP call {@LogInfo}", new
        {
            HttpMethod = HttpContext.Request.Method,
            RequestPath = HttpContext.Request.Path.ToString(),
            StatusCode = 204,
            Timestamp = DateTimeOffset.UtcNow
        });
        
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
        
        _logger.LogInformation("HTTP call {@LogInfo}", new
        {
            HttpMethod = HttpContext.Request.Method,
            RequestPath = HttpContext.Request.Path.ToString(),
            StatusCode = 204,
            Timestamp = DateTimeOffset.UtcNow
        });
        
        return NoContent();
    }
    
}