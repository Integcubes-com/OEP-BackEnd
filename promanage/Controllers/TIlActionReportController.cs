using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TIlActionReportController : ControllerBase
    {
        private readonly DAL _context;
        public TIlActionReportController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet("getReport")]
        public async Task<IActionResult> GetReports()
        {
            try
            {
                List<TilActionReportDto> results = new List<TilActionReportDto>();
                results = await _context.TilActionReportDto.FromSqlRaw(
               "select * from GetActionTrackerDetails()")
               .ToListAsync();

                var groupedBy = results.GroupBy(u => new
                {
                    u.equipmentId,
                    u.unit,
                    u.siteId,
                    u.siteName,
                    u.regionId,
                    u.regionTitle,
                    u.clusterId,
                    u.clusterTitle,
                    u.tilNumber
                }).Select(group => new 
                {
                    equipmentId = group.Key.equipmentId,
                    unit = group.Key.unit,
                    siteId = group.Key.siteId,
                    siteName = group.Key.siteName,
                    regionId = group.Key.regionId,
                    regionTitle = group.Key.regionTitle,
                    clusterId = group.Key.clusterId,
                    clusterTitle = group.Key.clusterTitle,
                    tilNumber = group.Key.tilNumber,
                    //statusId = group.Select(u => u.statusId).ToList(),
                    //statusTitle = group.Select(u => u.statusTitle).ToList(),
                    statusId = group.All(u => u.statusId == 3) ? 3 : 2,
                    statusTitle = group.All(u => u.statusTitle == "Closed") ? "Closed" : "Open",
                }).ToList();

             

                return Ok(groupedBy);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
