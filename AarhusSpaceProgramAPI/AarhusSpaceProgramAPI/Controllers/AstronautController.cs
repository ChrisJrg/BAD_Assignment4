using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using AarhusSpaceProgramAPI.Models;

namespace AarhusSpaceProgramAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AstronautController : ControllerBase
{
    private readonly  ApplicationDbContext _context;
    private readonly ILogger<AstronautController> _logger;

    public AstronautController(ApplicationDbContext context, ILogger<AstronautController> logger)
    {
        _context = context;
        _logger = logger;
    }


    [HttpGet("space-experience")]
    public async Task<ActionResult<IEnumerable<AstronautExperienceDto>>> GetAstronautsBySpaceExperience()
    {
        var astronauts = await _context.Astronauts
            .OrderByDescending(a => a.EXPInSpace)
            .Select(a => new AstronautExperienceDto
            {
                AstronautId =  a.AstronautId,
                Name = a.Name,
                Rank = a.Rank,
                EXPInSpace = a.EXPInSpace,
            }).ToListAsync();
        
        return Ok(astronauts);
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
    public async Task<ActionResult<AstronautDto>> CreateAstronaut([FromBody] AstronautDto dto)
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
    public async Task<IActionResult> UpdateAstronaut(int id, [FromBody] AstronautDto dto)
    {
        var  astronaut = await _context.Astronauts.FindAsync(id);
        if (astronaut == null)
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
        
        astronaut.Name = dto.Name;  
        astronaut.HireDate = dto.HireDate;
        astronaut.PayGrade = dto.PayGrade;
        astronaut.Rank = dto.Rank;
        astronaut.EXPInSim = dto.EXPInSim;
        astronaut.EXPInSpace = dto.EXPInSpace;
        
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
    public async Task<IActionResult> DeleteAstronaut(int id)
    {
        var astronaut = await _context.Astronauts.FindAsync(id);
        if (astronaut == null)
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

        _context.Astronauts.Remove(astronaut);
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