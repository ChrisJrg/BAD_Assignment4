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
        public async Task<ActionResult<MissionDto>> CreateMission(string missionName, DateTime launchDate, double duration, string status, string type, int rocketId, int launchPadId, int managerId, int targetBodyId)
        {
            var mission = new Mission
            {
                MissionName = missionName,
                LaunchDate = launchDate,
                Duration = duration,
                Status = status,
                Type = type,
                RocketId = rocketId,
                LaunchPadId =  launchPadId,
                ManagerId = managerId,
                TargetBodyId = targetBodyId
            };
            
            _context.Missions.Add(mission);
            await _context.SaveChangesAsync();

            var resultDto = new MissionDto
            {
                MissionName = missionName,
                LaunchDate = launchDate,
                Duration = duration,
                Status = status,
                Type = type,
                RocketId = rocketId,
                LaunchpPadId = launchPadId,
                ManagerId = managerId,
                TargetBodyId = targetBodyId
            };
            return CreatedAtAction(missionName,new {mission.MissionName}, resultDto); 
        }
    }
}
