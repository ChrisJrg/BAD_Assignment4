using AarhusSpaceProgramAPI.Data;
using Microsoft.AspNetCore.Mvc;
using AarhusSpaceProgramAPI.Models;
using AarhusSpaceProgramAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace AarhusSpaceProgramAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExperimentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public ExperimentController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [Authorize(Policy = "ExperimentCrud")]
    [HttpPost]
    public async Task<ActionResult<ExperimentwdDto>> CreateExperiment([FromForm] ExperimentwdDto dto)
    {
        var experiment = new Experiment
        {
            ExperimentName = dto.ExperimentName,
            Description = dto.Description,
            MissionId = dto.MissionId,
            CreatedAt = DateTime.Now,
            ScientistId = dto.ScientistId,
        };
        _context.Experiment.Add(experiment);
        await _context.SaveChangesAsync();

        var resultDto = new ExperimentwdDto
        {
            ExperimentName = experiment.ExperimentName,
            Description = experiment.Description,
            MissionId = experiment.MissionId,
            ScientistId = experiment.ScientistId,
        };

        return Ok(resultDto);
    }

    [Authorize(Roles = "Astronaut,Scientist,Manager")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExperimentDto>>> GetExperiments()
    {
        var experiments = await _context.Experiment
            .Select(e => new ExperimentDto
            {
                ExperimentName = e.ExperimentName,
                Description = e.Description,
                CreatedAt = DateTime.Now,
                MissionId = e.MissionId,
                ScientistId =  e.ScientistId,
            }).ToListAsync();

        return Ok(experiments);
    }

    [Authorize(Roles = "Scientist,Manager")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ExperimentDto>> UpdateExperiment(int id, [FromBody] ExperimentDto dto)
    {
        var experiment = await _context.Experiment.FindAsync(id);
        if (experiment == null)
        {
            return NotFound();
        }
        
        if (id != dto.ExperimentId)
        {
            return BadRequest("Route id and ExperimentId must match.");
        }
        
        experiment.ExperimentName = dto.ExperimentName;
        experiment.Description = dto.Description;
        experiment.MissionId = dto.MissionId;
        experiment.ScientistId = dto.ScientistId;
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    
    [Authorize(Roles = "Scientist,Manager")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ExperimentDto>> DeleteExperiment(int id)
    {
        var experiment = await _context.Experiment.FindAsync(id);
        if (experiment == null)
        {
            return NotFound();
        }
        
        _context.Experiment.Remove(experiment);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}