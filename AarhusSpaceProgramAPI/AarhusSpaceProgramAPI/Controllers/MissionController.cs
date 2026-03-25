using Microsoft.AspNetCore.Mvc;
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
        public MissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<MissionDto>> CreateMission([FromBody] MissionPostDto missionDto)
        {
            var mission = new Mission
            {
                MissionName = missionDto.MissionName,
                LaunchDate = missionDto.LaunchDate,
                Duration = missionDto.Duration,
                Status = missionDto.Status,
                Type = missionDto.Type, 
            };
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
            
            return  Ok(missions);
        }
        
        
        [HttpPut]
        public async Task<IActionResult> UpdateMissionAssignAstronaut(int missionId, int astronautId)
        {
            var mission = await _context.Missions
                .Include(i => i.Astronauts)
                .SingleOrDefaultAsync(m => m.MissionId == missionId);
            
            var astronaut = await _context.Astronauts
                .SingleOrDefaultAsync(a => a.AstronautId == astronautId);
            
            mission.Astronauts.Add(astronaut);
            await _context.SaveChangesAsync();
            
            return Ok(mission);
        }
    }
}
