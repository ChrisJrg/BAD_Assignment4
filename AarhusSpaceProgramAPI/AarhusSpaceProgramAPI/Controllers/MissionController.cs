using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using AarhusSpaceProgramAPI.Models;
using AarhusSpaceProgramAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;


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

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult<MissionDto>> CreateMission([FromBody] MissionDto missionDto)
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

            var missionCheck = _context.Missions
                .Where(m => m.LaunchDate == mission.LaunchDate)
                .Where(m => m.LaunchPadId == mission.LaunchPadId)
                .ToList();

            if (missionCheck.Count != 0)
            {
                LogHttpCall(409);
                return Conflict("A launchpad cannot be launched from twice in one day.");
            }
            
            
            
            if (!Statuses.Contains(mission.Status))
            {
                LogHttpCall(409);
                return Conflict("Status must be in the pre approved list: Created, Budgeted, Approved, Planned, Active Completed, Aborted, Failed");
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
            
            LogHttpCall(200);

            
            return Ok(resultDto);
        }
        
        
        [AllowAnonymous]
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
        
        
        [Authorize(Roles = "Astronaut,Manager")]
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

        [Authorize(Roles = "Astronaut,Manager")]
        [HttpGet("mission-experiments")]
        public async Task<ActionResult<IEnumerable<MissionExperimentsDto>>> GetMissionExperiments()
        {
            var missions = await _context.Missions
                .Select(m => new MissionExperimentsDto
                {
                    MissionName = m.MissionName,
                    Experiments = m.Experiments
                }).ToListAsync();

            return Ok(missions);
        }

        
        [Authorize(Roles = "Manager")]
        [HttpDelete("{missionId}")]
        public async Task<IActionResult> DeleteMission(int missionId)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null)
            {
                LogHttpCall(404);

                return NotFound();
            }

            _context.Missions.Remove(mission);
            await _context.SaveChangesAsync();
            
            LogHttpCall(204);

        
            return NoContent();
        }
        
        [Authorize(Roles = "Manager")]
        [HttpPut("AssignAstronaut/{astronautId}")]
        public async Task<IActionResult> UpdateMissionAssignAstronaut(int missionId, int astronautId)
        {
            var mission = await _context.Missions
                .Include(i => i.Astronauts)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            if (mission == null)
            {
                LogHttpCall(409);

                return Conflict("No mission is assigned this ID");
            }
            
            
            var astronaut = await _context.Astronauts
                .SingleOrDefaultAsync(a => a.AstronautId == astronautId);
            if (astronaut == null)
            {
                
                LogHttpCall(409);

                
                return Conflict("No astronaut is assigned this ID");
            }
            
            
            mission.Astronauts.Add(astronaut);
            await _context.SaveChangesAsync();
            
            LogHttpCall(200);
            
            return Ok(mission);
        }
        
        
        [Authorize(Roles = "Manager")]
        [HttpPut("RemoveAstronaut/{astronautId}")]
        public async Task<IActionResult> UpdateMissionRemoveAstronaut(int missionId, int astronautId)
        {
            var mission = await _context.Missions
                .Include(i => i.Astronauts)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            if (mission == null)
            {
                LogHttpCall(409);

                return Conflict("No mission is assigned this ID");
            }
            
            
            var astronaut = await _context.Astronauts
                .SingleOrDefaultAsync(a => a.AstronautId == astronautId);
            if (astronaut == null)
            {
                LogHttpCall(409);

                return Conflict("No astronaut is assigned this ID");
            }
            
            
            mission.Astronauts.Remove(astronaut);
            await _context.SaveChangesAsync();
            
            LogHttpCall(204);

            
            return NoContent();
        }
        
        [Authorize(Roles = "Manager")]
        [HttpPut("AssignScientist/{scientistId}")]
        public async Task<IActionResult> UpdateMissionAssignScientist(int missionId, int scientistId)
        {
            var mission = await _context.Missions
                .Include(i => i.Scientists)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            if (mission == null)
            {
                LogHttpCall(409);

                return Conflict("No mission is assigned this ID");
            }
            
            var scientist = await _context.Scientists
                .SingleOrDefaultAsync(a => a.ScientistId == scientistId);
            if (scientist == null)
            {
                LogHttpCall(409);

                return Conflict("No scientist is assigned this ID");
            }
            
            mission.Scientists.Add(scientist);
            await _context.SaveChangesAsync();
            LogHttpCall(204);

            
            return NoContent();
        }
        
        [Authorize(Roles = "Manager")]
        [HttpPut("RemoveScientist/{scientistId}")]
        public async Task<IActionResult> UpdateMissionRemoveScientist(int missionId, int scientistId)
        {
            var mission = await _context.Missions
                .Include(i => i.Scientists)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            if (mission == null)
            {
                LogHttpCall(409);

                return Conflict("No mission is assigned this ID");
            }
            
            var scientist = await _context.Scientists
                .SingleOrDefaultAsync(a => a.ScientistId == scientistId);
            if (scientist == null)
            {
                LogHttpCall(409);

                return Conflict("No scientist is assigned this ID");
            }
            
            mission.Scientists.Remove(scientist);
            await _context.SaveChangesAsync();
            
            LogHttpCall(204);

            
            return NoContent();
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("StatusUpdate/{missionId}")]
        public async Task<IActionResult> UpdateMissionStatus(string status, int missionId)
        {
            if (string.IsNullOrEmpty(status))
            {
                LogHttpCall(400);

                return BadRequest("Status is required");
            }

            if (!Statuses.Contains(status))
            {
                LogHttpCall(409);
                return Conflict("Status must be in the pre approved list: Created, Budgeted, Approved, Planned, Active Completed, Aborted, Failed");
            } 
            
            var mission = await _context.Missions
                .Include(i => i.Status)
                .Where(m => m.MissionId == missionId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.Status, status));

            await _context.SaveChangesAsync();
            
            LogHttpCall(200);

            return Ok(mission);
        }
        
        [Authorize(Roles = "Manager")]
        [HttpPut("UpdateMission/{missionId}")]
        public async Task<IActionResult> UpdateMission(int missionId, [FromBody] MissionDto missionDto)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null)
            {
                LogHttpCall(404);

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
            
            
            if (!Statuses.Contains(mission.Status))
            {
                LogHttpCall(409);
                return Conflict("Status must be in the pre approved list: Created, Budgeted, Approved, Planned, Active Completed, Aborted, Failed");
            } 
            

            _context.Entry(mission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            LogHttpCall(204);
            
            return NoContent();
        }
        
        
        [Authorize(Roles = "Astronaut,Manager")]
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
        
        [Authorize(Roles = "Astronaut,Manager")]
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

            LogHttpCall(200);
            return Ok(mission);
            

        }
        
    }
}
