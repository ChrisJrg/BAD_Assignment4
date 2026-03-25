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
    private readonly  ILogger<ScientistController> _logger;

    public ScientistController(ApplicationDbContext context,  ILogger<ScientistController> logger)
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

        LogHttpCall(200);
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
        
        LogHttpCall(200);

        
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScientist(int id, [FromForm]ScientistDto dto)
    {
        var  scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null)
        {
            LogHttpCall(404);

            return NotFound();
        }

        scientist.Name = dto.Name;
        scientist.HireDate = dto.HireDate;
        scientist.Title = dto.Title;
        scientist.Specialty = dto.Specialty;
        
        _context.Entry(scientist).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        LogHttpCall(204);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScientist(int id)
    {
        var scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null)
        {
            LogHttpCall(204);

            return NotFound();
        }

        _context.Scientists.Remove(scientist);
        await _context.SaveChangesAsync();
        
        LogHttpCall(204);

        
        return NoContent();
    }
    
}