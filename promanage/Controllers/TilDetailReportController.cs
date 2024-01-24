using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ActionTrakingSystem.Controllers
{

    public class TilDetailReportController : BaseAPIController
    {
        private readonly DAL _context;
        public TilDetailReportController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet("getReport")]
        public async Task<IActionResult> GetReports()
        {
            try
            {
                List<TilDetailReportDto> results = new List<TilDetailReportDto>();
                    results = await _context.TilDetailReport.FromSqlRaw(
                   "EXEC dbo.GetDetailedCountsAndTotal")
                   .ToListAsync();
                return Ok(results);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
