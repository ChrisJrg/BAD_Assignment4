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
    private readonly ILogger<ManagerController> _logger;

    public RocketController(ApplicationDbContext context,  ILogger<ManagerController> logger)
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

        LogHttpCall(200);
        return Ok(rockets);
    }

    [HttpPost]
    public async Task<ActionResult<RocketDto>> CreateRocket([FromBody]RocketDto dto)
    {
        var rocket = new Rocket
        {
            Model = dto.Model,
            Weight = dto.Weight,
            CrewCapacity = dto.CrewCapacity,
            Stages = dto.Stages,
            FuelCapacity =  dto.FuelCapacity,
            PayloadCapacity =  dto.PayloadCapacity,
        };

        if (rocket.Weight < 0)
        {
            LogHttpCall(409);
            return Conflict("Weight cannot be negative");
        }
        
        
        _context.Rockets.Add(rocket);
        await _context.SaveChangesAsync();

        var resultDto = new RocketDto
        {
            Model = rocket.Model,
            Weight = rocket.Weight,
            CrewCapacity = rocket.CrewCapacity,
            Stages = rocket.Stages,
            FuelCapacity =  rocket.FuelCapacity,
            PayloadCapacity =  rocket.PayloadCapacity,
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
    public async Task<IActionResult> UpdateRocket(int id, [FromBody]RocketDto dto)
    {
        var  rocket = await _context.Rockets.FindAsync(id);
        if (rocket == null)
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
        rocket.Model = dto.Model;
        rocket.Weight = dto.Weight;
        rocket.CrewCapacity = dto.CrewCapacity;
        rocket.Stages = dto.Stages;
        rocket.FuelCapacity =  dto.FuelCapacity;
        rocket.PayloadCapacity = dto.PayloadCapacity;
        
        if (rocket.Weight < 0) return Conflict("Weight cannot be negative");
        
        
        _context.Entry(rocket).State = EntityState.Modified;
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
    public async Task<IActionResult> DeleteRocket(int id)
    {
        var rocket = await _context.Rockets.FindAsync(id);
        if (rocket == null)
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

        _context.Rockets.Remove(rocket);
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