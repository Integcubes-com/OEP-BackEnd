using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class WH_WorkingHoursController : BaseAPIController
    {
        private readonly DAL _context;
        public WH_WorkingHoursController(DAL context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("getWorkingHour")]
        public async Task<IActionResult> GetWorkingHour(WH_WorkingHoursUserDto reg)
        {
            try
            {
                var workingEq = await (from a in _context.WH_StartingHours.Where(a => a.isDeleted == 0)
                                       join se in _context.SiteEquipment.Where(a => a.isDeleted == 0 && (reg.filter.equipmentId == -1 || a.equipmentId == reg.filter.equipmentId)) on a.unitId equals se.equipmentId
                                       join s in _context.Sites.Where(a => reg.filter.siteId == -1 || a.siteId == reg.filter.siteId) on se.siteId equals s.siteId
                                       join r in _context.Regions.Where(a => reg.filter.regionId == -1 || a.regionId == reg.filter.regionId) on s.regionId equals r.regionId
                                       join ts in _context.SitesTechnology.Where(a => a.isDeleted == 0) on s.siteId equals ts.siteId
                                       join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                       join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on ts.techId equals aut.technologyId
                                       select new
                                       {
                                           a.startingId,
                                           r.regionId,
                                           regionTitle = r.title,
                                           s.siteId,
                                           siteTitle = s.siteName,
                                           se.equipmentId,
                                           se.unit,
                                           a.startHours,
                                           a.startDate,
                                           s.onmContractExpiry
                                       }).Distinct().OrderByDescending(a=>a.startingId).ToListAsync();
                return Ok(workingEq);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("deleteWorkingHour")]
        public async Task<IActionResult> DeleteWorkingHour(WH_UserDto reg)
        {
            try
            {
                var workingEq = await (from a in _context.WH_StartingHours.Where(a => a.startingId == reg.result.startingId)
                                       select a
                                       ).FirstOrDefaultAsync();
                workingEq.isDeleted = 1;
                _context.SaveChanges();
                return Ok(workingEq);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("saveWorkingHour")]
        public async Task<IActionResult> SaveWorkingHour(WH_UserDto reg)
        {
            try
            {
                if(reg.result.startingId == -1)
                {
                    WH_StartingHours sh = new WH_StartingHours();
                    sh.startHours = reg.result.startHours;
                    sh.startDate = Convert.ToDateTime(reg.result.startDate);
                    sh.unitId = reg.result.equipmentId;
                    sh.isDeleted = 0;
                    sh.createdOn = DateTime.Now;
                    sh.createdBy = reg.userId;
                    _context.Add(sh);
                    _context.SaveChanges();
                    return Ok(sh);
                }
                else
                {
                    var sh = await (from a in _context.WH_StartingHours.Where(a => a.startingId == reg.result.startingId)
                                           select a
                       ).FirstOrDefaultAsync();
                    sh.startHours = reg.result.startHours;
                    sh.startDate = Convert.ToDateTime(reg.result.startDate);
                    sh.unitId = reg.result.equipmentId;
                    sh.modifiedOn = DateTime.Now;
                    sh.modifiedBy = reg.userId;
                    _context.SaveChanges();
                    return Ok(sh);

                }


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getYearlyWorkingHour")]
        public async Task<IActionResult> GetYearlyWorkingHour(WH_YearlyDro reg)
        {
            try
            {
                if (reg.typeId == 1)
                {
                    var monthlyHours = await (from a in _context.WH_MonthlyHours.Where(a => a.startingHourId == reg.result.startingId)
                                              select a
                                             ).ToListAsync();
                    int differenceInYears = reg.result.onmContractExpiry.Year - reg.result.startDate.Year;
                    List<int> uiqueYear = new List<int>();
                    var d = 0;
                    for (var za = 0; za < differenceInYears; za++)
                    {
                        d = reg.result.startDate.Year + za + 1;
                        uiqueYear.Add(d);
                        d = 0;
                    }

                    //var uiqueYear = monthlyHours.Select(a=>a.yearId).Where(a=>a > reg.result.startDate.Year).Distinct().ToList();
                    List<WH_YearlyModel> yealyList = new List<WH_YearlyModel>();
                    decimal yearltCount = 0;
                    decimal yearlyTotal = reg.result.startHours;
                    for (var i = 0; i < uiqueYear.Count; i++)
                    {
                        WH_YearlyModel yearlyModel = new WH_YearlyModel();

                        for (var j = 0; j < monthlyHours.Count; j++)
                        {
                            if (monthlyHours[j].yearId == uiqueYear[i])
                            {
                                yearltCount += monthlyHours[j].runningHour;
                            }
                        }

                        yearlyModel.runningHours = Math.Round(yearltCount,2);
                        yearlyModel.yearId = uiqueYear[i];
                        yearlyTotal += yearltCount;
                        yearlyModel.yearlyTotal = yearlyTotal;

                        yealyList.Add(yearlyModel);

                        // Reset yearltCount for the next iteration
                        yearltCount = 0;
                    }
                    var obj = new
                    {
                        uiqueYear,
                        yealyList,
                        startHours = reg.result.startHours,
                    };
                    return Ok(obj);
                }
                else
                {
                    var monthlyHours = await (from a in _context.WH_MonthlyHours.Where(a => a.startingHourId == reg.result.startingId && a.yearId <= reg.yearId)
                                              select new
                                              {
                                                  yearId = a.yearId,
                                                  runningHours = a.runningHour,
                                                  monthId = a.monthId
                                              }
                                             ).OrderBy(a => a.monthId).ToListAsync();
                    var calcStartingHours = monthlyHours.Select(a => new
                    {
                        a.runningHours,
                        a.yearId
                    }).Where(a => a.yearId < reg.yearId).ToList();
                    decimal monthlyTotal = reg.result.startHours;
                    for (var i = 0; i < calcStartingHours.Count; i++)
                    {
                        monthlyTotal += calcStartingHours[i].runningHours;
                    }
                    var startHours = monthlyTotal;

                    var monthlyHours2 = monthlyHours.Select(a => new
                    {
                        a.runningHours,
                        a.yearId,
                        a.monthId
                    }).Where(a => a.yearId == reg.yearId).ToList();
                    List<WH_MonthlyModel> monthlyList = new List<WH_MonthlyModel>();
                    decimal yearltCount = 0;
                    if (monthlyHours2.Count == 0)
                    {
                        for (var i = 0; i < 12; i++)
                        {
                            WH_MonthlyModel monthlyModel = new WH_MonthlyModel();
                            monthlyModel.runningHours = 0;
                            monthlyModel.monthId = i + 1;
                            monthlyModel.yearId = reg.yearId;
                            monthlyTotal += 0;
                            monthlyModel.monthlyTotal = monthlyTotal;
                            monthlyList.Add(monthlyModel);

                        }
                    }
                    else
                    {
                        for (var i = 0; i < monthlyHours2.Count; i++)
                        {
                            WH_MonthlyModel monthlyModel = new WH_MonthlyModel();
                            monthlyModel.runningHours = Math.Round(monthlyHours2[i].runningHours, 2);
                            monthlyModel.monthId = monthlyHours2[i].monthId;
                            monthlyModel.yearId = reg.yearId;
                            monthlyTotal += monthlyHours2[i].runningHours;
                            monthlyModel.monthlyTotal = monthlyTotal;
                            monthlyList.Add(monthlyModel);

                        }
                    }

                    var obj = new
                    {
                        yealyList = monthlyList,
                        startHours = startHours
                    };
                    return Ok(obj);
                }


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getSites")]
        public async Task<IActionResult> GetSites(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1))
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                   join eq in _context.SiteEquipment.Where(a=>a.isDeleted == 0) on s.siteId equals eq.siteId
                                   join ot in _context.WH_SiteNextOutages.Where(a => a.isDeleted == 0) on eq.equipmentId equals ot.equipmentId
                                   join dd in _context.WH_StartingHours.Where(a=>a.isDeleted == 0) on eq.equipmentId equals dd.unitId
                                   join cc in _context.WH_MonthlyHours on dd.startingId equals cc.startingHourId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName,
                                   }
                                 ).Distinct().OrderBy(a => a.siteTitle).ToListAsync();

                return Ok(sites);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveHours")]
        public async Task<IActionResult> SaveWorkingHour(WH_SaveHours reg)
        {
            try
            {

                if (reg.typeId == 1)
                {
                    for (var i = 0; i < reg.yearlyResult.Length; i++)
                    {
                        var sql = "DELETE FROM WH_MonthlyHours WHERE startingHourId = @startingHourId AND yearId = @yearId";
                                                SqlParameter[] parameters = {
                            new SqlParameter("@startingHourId", reg.result.startingId),
                            new SqlParameter("@yearId", reg.yearlyResult[i].yearId)
                        };

                        _context.Database.ExecuteSqlCommand(sql, parameters);
                        for (var j = 0; j < 12; j++)
                        {
                            WH_MonthlyHours ys = new WH_MonthlyHours();
                            ys.monthId = j + 1;
                            ys.startingHourId = reg.result.startingId;
                            ys.yearId = reg.yearlyResult[i].yearId;
                            ys.runningHour = (reg.yearlyResult[i].runningHours / 12);
                            ys.createdBy = reg.userId;
                            ys.createdOn = DateTime.Now;    
                            _context.Add(ys);
                            _context.SaveChanges();
                        }
                    }
                    return Ok();
                }
                else
                {
                    var sql = "DELETE FROM WH_MonthlyHours WHERE startingHourId = @startingHourId AND yearId = @yearId";
                    SqlParameter[] parameters = {
                            new SqlParameter("@startingHourId", reg.result.startingId),
                            new SqlParameter("@yearId", reg.yearlyResult[0].yearId)
                        };
                    _context.Database.ExecuteSqlCommand(sql, parameters);

                    for (var i = 0; i < reg.yearlyResult.Length; i++)
                    {
                            WH_MonthlyHours ys = new WH_MonthlyHours();
                            ys.monthId = reg.yearlyResult[i].monthId;
                            ys.startingHourId = reg.result.startingId;
                            ys.yearId = reg.yearlyResult[i].yearId;
                            ys.runningHour = (reg.yearlyResult[i].runningHours);
                            ys.createdBy = reg.userId;
                            ys.createdOn = DateTime.Now;
                            _context.Add(ys);
                            _context.SaveChanges();
                        
                    }
                    return Ok();
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("getTimeLine")]
        public async Task<IActionResult> GetTimeLine(WH_TimeLineDto reg)
        {
            try
            {
                var timeline = await (from a in _context.SiteEquipment.Where(a => a.siteId == reg.siteId)
                                      //join ot in _context.WH_SiteNextOutages.Where(a => a.isDeleted == 0) on a.equipmentId equals ot.equipmentId
                                      join s in _context.Sites.Where(a=>a.isDeleted == 0) on a.siteId equals s.siteId
                                      join sh in _context.WH_StartingHours.Where(a => a.isDeleted == 0) on a.equipmentId equals sh.unitId
                                      join mh in _context.WH_MonthlyHours on sh.startingId equals mh.startingHourId
                                      //join ot in _context.WH_SiteNextOutages on a.equipmentId equals ot.equipmentId into all
                                      //from ott in all.DefaultIfEmpty()
                                      //join it in _context.WH_ISiteOutages on ott.outageId equals it.outageId into all2
                                      //from itt in all2.DefaultIfEmpty()
                                      select new
                                      {
                                          a.siteId,
                                          a.equipmentId,
                                          a.unit,
                                          sh.startDate,
                                          sh.startHours,
                                          mh.yearId,
                                          mh.runningHour,
                                          mh.monthId,
                                          s.onmContractExpiry,
                                          //ott.outageId,
                                          //itt.outageTitle,
                                      }).ToListAsync();

                //var groupedHours = timeline.GroupBy(s => s.equipmentId).Select(g => new
                //{
                //    g.FirstOrDefault().equipmentId,
                //    siteId = g.FirstOrDefault().siteId,
                //    g.FirstOrDefault().unit,
                //    g.FirstOrDefault().startDate,
                //    g.FirstOrDefault().yearId,
                //    g.FirstOrDefault().runningHour,
                //    g.FirstOrDefault().monthId,
                //}).ToList();

                var outages = await (from a in _context.SiteEquipment.Where(a => a.siteId == reg.siteId)
                                      //join sh in _context.WH_StartingHours.Where(a => a.isDeleted == 0) on a.equipmentId equals sh.unitId
                                      //join mh in _context.WH_MonthlyHours on sh.startingId equals mh.startingHourId
                                      join ot in _context.WH_SiteNextOutages.Where(a=>a.isDeleted == 0) on a.equipmentId equals ot.equipmentId 
                                      join it in _context.WH_ISiteOutages on ot.outageId equals it.outageId 
                                      select new
                                      {
                                          a.siteId,
                                          a.equipmentId,
                                          a.unit,
                                          //sh.startDate,
                                          //sh.startHours,
                                          //mh.yearId,
                                          //mh.runningHour,
                                          //mh.monthId,
                                          ot.outageId,
                                          it.outageTitle,
                                          ot.runningHours,
                                          ot.nextOutageDate,
                                          it.colorCode,
                                          counter = -1
                                      }).ToListAsync();
                //var groupedOutages = outages.GroupBy(s => s.equipmentId).Select(g => new
                //{
                //    g.FirstOrDefault().equipmentId,
                //    siteId = g.FirstOrDefault().siteId,
                //    g.FirstOrDefault().unit,
                //    g.FirstOrDefault().outageId,
                //    g.FirstOrDefault().outageTitle
                //}).ToList();

                var obj = new
                {
                    timeline,
                    outages
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
