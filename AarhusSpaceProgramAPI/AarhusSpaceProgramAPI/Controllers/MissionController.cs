using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using AarhusSpaceProgramAPI.Models;

namespace AarhusSpaceProgramAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private string[] Statuses = ["Created", "Budgeted", "Approved", "Planned", "Active", "Completed", "Aborted", "Failed"];
        private readonly ILogger<MissionController> _logger;
        public MissionController(ApplicationDbContext context, ILogger<MissionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<MissionDto>> CreateMission([FromForm] MissionDto missionDto)
        {
            var mission = new Mission
            {
                MissionName = missionDto.MissionName,
                LaunchDate = missionDto.LaunchDate,
                Duration = missionDto.Duration,
                Status = missionDto.Status,
                Type = missionDto.Type, 
                RocketId = missionDto.RocketId,
                LaunchPadId = missionDto.LaunchPadId,
                ManagerId = missionDto.ManagerId,
                TargetBodyId = missionDto.TargetBodyId
            };
            if (!Statuses.Contains(mission.Status))
            {
                throw new ArgumentException();
            }
            _context.Missions.Add(mission);
            await _context.SaveChangesAsync();

            var resultDto = new MissionDto
            {
                MissionName = mission.MissionName,
                LaunchDate = mission.LaunchDate,
                Duration = mission.Duration,
                Status = mission.Status,
                Type = mission.Type,
            };
            
            _logger.LogInformation("HTTP call {@LogInfo}", new
            {
                HttpMethod = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path.ToString(),
                StatusCode = 204,
                Timestamp = DateTimeOffset.UtcNow
            });
            
            return Ok(resultDto);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MissionDto>>> GetMissions()
        {
            var mission = await _context.Missions
                .Select(m => new MissionDto
                {
                    MissionId =  m.MissionId,
                    MissionName = m.MissionName,
                    LaunchDate = m.LaunchDate,
                    Duration =  m.Duration,
                    Status =  m.Status,
                    Type = m.Type,
                    RocketId = m.RocketId,
                    LaunchPadId = m.LaunchPadId,
                    ManagerId = m.ManagerId,
                    TargetBodyId = m.TargetBodyId
                }).ToListAsync();
        
            return Ok(mission);
        }
        
        [HttpGet("mission-overview")]
        public async Task<ActionResult<IEnumerable<MissionOverviewDto>>> GetMissionOverview()
        {
            var missions = await _context.Missions
                .Include(m => m.LaunchPad)
                .Include(m => m.Manager)
                .Include(m => m.Rocket)
                .Include(m => m.TargetBody)
                .Select(m => new MissionOverviewDto
                {
                    MissionName =  m.MissionName,
                    LaunchDate = m.LaunchDate,
                    ManagerName = m.Manager.Name,
                    RocketModel = m.Rocket.Model,
                    LaunchPadLocation = m.LaunchPad.Location,
                    TargetBodyName = m.TargetBody.Name
                }).ToListAsync();
            
            return Ok(missions);
        }

        [HttpDelete("{missionId}")]
        public async Task<IActionResult> DeleteMission(int missionId)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null)
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

            _context.Missions.Remove(mission);
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
        
        [HttpPut("AssignAstronaut/{astronautId}")]
        public async Task<IActionResult> UpdateMissionAssignAstronaut(int missionId, int astronautId)
        {
            var mission = await _context.Missions
                .Include(i => i.Astronauts)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            if (mission == null)
            {
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 409,
                    Timestamp = DateTimeOffset.UtcNow
                });
                return Conflict("No mission is assigned this ID");
            }
            
            
            var astronaut = await _context.Astronauts
                .SingleOrDefaultAsync(a => a.AstronautId == astronautId);
            if (astronaut == null)
            {
                
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 409,
                    Timestamp = DateTimeOffset.UtcNow
                });
                
                return Conflict("No astronaut is assigned this ID");
            }
            
            
            mission.Astronauts.Add(astronaut);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("HTTP call {@LogInfo}", new
            {
                HttpMethod = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path.ToString(),
                StatusCode = 200,
                Timestamp = DateTimeOffset.UtcNow
            });
            
            return Ok(mission);
        }
        
        [HttpPut("RemoveAstronaut/{astronautId}")]
        public async Task<IActionResult> UpdateMissionRemoveAstronaut(int missionId, int astronautId)
        {
            var mission = await _context.Missions
                .Include(i => i.Astronauts)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            if (mission == null)
            {
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 409,
                    Timestamp = DateTimeOffset.UtcNow
                });
                return Conflict("No mission is assigned this ID");
            }
            
            
            var astronaut = await _context.Astronauts
                .SingleOrDefaultAsync(a => a.AstronautId == astronautId);
            if (astronaut == null)
            {
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 409,
                    Timestamp = DateTimeOffset.UtcNow
                });
                return Conflict("No astronaut is assigned this ID");
            }
            
            
            mission.Astronauts.Remove(astronaut);
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
        
        [HttpPut("AssignScientist/{scientistId}")]
        public async Task<IActionResult> UpdateMissionAssignScientist(int missionId, int scientistId)
        {
            var mission = await _context.Missions
                .Include(i => i.Scientists)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            if (mission == null)
            {
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 409,
                    Timestamp = DateTimeOffset.UtcNow
                });
                return Conflict("No mission is assigned this ID");
            }
            
            var scientist = await _context.Scientists
                .SingleOrDefaultAsync(a => a.ScientistId == scientistId);
            if (scientist == null)
            {
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 409,
                    Timestamp = DateTimeOffset.UtcNow
                });
                return Conflict("No scientist is assigned this ID");
            }
            
            mission.Scientists.Add(scientist);
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
        
        [HttpPut("RemoveScientist/{scientistId}")]
        public async Task<IActionResult> UpdateMissionRemoveScientist(int missionId, int scientistId)
        {
            var mission = await _context.Missions
                .Include(i => i.Scientists)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            if (mission == null)
            {
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 409,
                    Timestamp = DateTimeOffset.UtcNow
                });
                return Conflict("No mission is assigned this ID");
            }
            
            var scientist = await _context.Scientists
                .SingleOrDefaultAsync(a => a.ScientistId == scientistId);
            if (scientist == null)
            {
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 409,
                    Timestamp = DateTimeOffset.UtcNow
                });
                return Conflict("No scientist is assigned this ID");
            }
            
            mission.Scientists.Remove(scientist);
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

        [HttpPut("StatusUpdate/{missionId}")]
        public async Task<IActionResult> UpdateMissionStatus(string status, int missionId)
        {
            if (string.IsNullOrEmpty(status))
            {
                _logger.LogInformation("HTTP call {@LogInfo}", new
                {
                    HttpMethod = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path.ToString(),
                    StatusCode = 400,
                    Timestamp = DateTimeOffset.UtcNow
                });
                return BadRequest("Status is required");
            }
            if (!Statuses.Contains(status)) throw new ArgumentException();
            
            var mission = await _context.Missions
                .Include(i => i.Status)
                .Where(m => m.MissionId == missionId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.Status, status));

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("HTTP call {@LogInfo}", new
            {
                HttpMethod = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path.ToString(),
                StatusCode = 200,
                Timestamp = DateTimeOffset.UtcNow
            });
            return Ok(mission);
        }
        
        [HttpPut("UpdateMission/{missionId}")]
        public async Task<IActionResult> UpdateMission(int missionId, [FromForm] MissionDto missionDto)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null)
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
                
            mission.MissionName = missionDto.MissionName;
            mission.LaunchDate = missionDto.LaunchDate;
            mission.Duration =  missionDto.Duration;
            mission.Status =  missionDto.Status;
            mission.Type = missionDto.Type;
            mission.RocketId = missionDto.RocketId;
            mission.LaunchPadId = missionDto.LaunchPadId;
            mission.ManagerId = missionDto.ManagerId;
            mission.TargetBodyId = missionDto.TargetBodyId;

            _context.Entry(mission).State = EntityState.Modified;
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
        
        [HttpGet("AssignedToMission/{missionId}")]
        public async Task<IActionResult> AssignedToMissions(int missionId)
        {
            var mission = await _context.Missions    
                .Where(m =>  m.MissionId == missionId)
                .Select(m => new MissionAstSciDto()
            {
                MissionName = m.MissionName,
                Astronauts =  m.Astronauts,
                Scientists =  m.Scientists
            }).ToListAsync();
        
            return Ok(mission);
        }
        
        [HttpGet("MissionsAtTargetBody/{planetId}")]
        public async Task<IActionResult> MissionsAtTargetBody(int planetId)
        {
            var mission = await _context.Missions    
                .Where(m =>  m.TargetBodyId == planetId)
                .Select(m => new MissionPlanetDto()
                {
                    MissionName = m.MissionName,
                    TargetBodyId = m.TargetBodyId,
                    TargetBody = m.TargetBody
                }).ToListAsync();
        
            return Ok(mission);
            

        }
        
    }
}
