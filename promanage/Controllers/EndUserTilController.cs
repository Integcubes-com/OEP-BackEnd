using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class EndUserTilController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EndUserTilController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpPost("getAttachedFiles")]
        public async Task<IActionResult> GetAttachedFiles(tatFileListDto reg)
        {
            try
            {

                var list = await (from a in _context.TILActiontrackerFile.Where(a => a.tapId == reg.tapId && a.isDeleted == 0 && a.equipId == reg.equipId)
                                  join b in _context.TILActionTracker on new { a.tapId , a.equipId } equals new { b.tapId , equipId = b.siteEquipmentId }
                                  select new
                                  {
                                      tapId = a.tapId,
                                      equipId = a.equipId,
                                      filePath = a.reportPath,
                                      name = a.reportName,
                                      a.remarks,
                                      docId = a.tapfId,
                                  }).Distinct().ToListAsync();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //[Authorize]
        //[HttpPost("uploadFile")]
        //public async Task<IActionResult> uploadFiles([FromForm] tilFileEnduser reg)
        //{
        //    try
        //    {
        //        if (reg.tilReport != null)
        //        {
        //            string tilFileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.tilReport.FileName);
        //            string tilfilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", tilFileName);
        //            using (var stream = new FileStream(tilfilePath, FileMode.Create))
        //            {
        //                await reg.tilReport.CopyToAsync(stream);
        //            }
        //            string tilFileUrl = $"~/uploads/{tilFileName}";
        //            _context.Database.ExecuteSqlCommand("DELETE FROM TILActiontrackerFile WHERE iatId = @tapId", new SqlParameter("tapId", Convert.ToInt32(reg.tatId)));
        //            TILActiontrackerFile fileIR = new TILActiontrackerFile();
        //            fileIR.reportPath = tilFileUrl;
        //            fileIR.tapId = Convert.ToInt32(reg.tatId);
        //            fileIR.reportName = reg.tilReport.FileName;
        //            fileIR.createdBy = Convert.ToInt32(reg.userId);
        //            fileIR.createdOn = DateTime.Now;
        //            fileIR.isDeleted = 0;
        //            _context.Add(fileIR);
        //        }
        //        _context.SaveChanges();
        //        return Ok();
        //    }
        //    catch (Exception E)
        //    {
        //        return BadRequest(E.Message);
        //    }
        //}
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {
                var statusList = await (from s in _context.TAStatus.Where(a => a.isDeleted == 0)
                                        select new
                                        {
                                            statusId = s.tasId,
                                            statustitle = s.title

                                        }).ToArrayAsync();
                var equipment = await (
                                       from e in _context.SiteEquipment.Where(a => a.isDeleted == 0)
                                       select new
                                       {
                                           siteTitle = e.siteUnit,
                                           e.equipmentId,
                                           unit = e.unit,
                                       }).Distinct().ToListAsync();
                var budget = await (from t in _context.TABudget.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        t.budgetId,
                                        t.budgetName,
                                    }).Distinct().ToListAsync();
                var priority = await (from t in _context.TAPPriority.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          t.priorityTitle,
                                          t.priorityId,
                                      }).Distinct().ToListAsync();
                var budgetSource = await (from t in _context.TABudgetSource.Where(a => a.isDeleted == 0)
                                          select new
                                          {
                                              t.budgetSourceId,
                                              t.budgetSourceTitle,
                                          }).Distinct().ToListAsync();

                var part = await (from t in _context.TAParts.Where(a => a.isDeleted == 0)
                                  select new
                                  {
                                      t.partId,
                                      t.partTitle,
                                  }).Distinct().ToListAsync();
                var finalImplementation = await (from t in _context.TAFinalImplementation.Where(a => a.isDeleted == 0)
                                                 select new
                                                 {
                                                     t.finalImpId,
                                                     t.finalImpTitle,
                                                 }).Distinct().ToListAsync();
                var sapPlaning = await (from t in _context.TASapPlanning.Where(a => a.isDeleted == 0)
                                        select new
                                        {
                                            t.sapPlanningTitle,
                                            t.sapPlanningId,
                                        }).Distinct().ToListAsync();
                var evidence = await (from t in _context.TAEvidence.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          t.evidenceId,
                                          t.evidenceTitle,
                                      }).Distinct().ToListAsync();
                var oemSeverity = await (from tc in _context.TILOEMSeverity.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             tc.oemSeverityId,
                                             tc.oemSeverityTitle
                                         }).ToListAsync();
                //var oemSeverityTiming = await (from tc in _context.TILOEMTimimgCode.Where(a => a.isDeleted == 0)
                //                               select new
                //                               {
                //                                   tc.timingId,
                //                                   tc.timingCode
                //                               }).ToListAsync();

                var tilFocus = await (from tc in _context.TILFocus.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          tc.focusId,
                                          tc.focusTitle
                                      }).ToListAsync();
                var obj = new
                {
                    priority,
                    budgetSource,
                    budget,
                    part,
                    finalImplementation,
                    evidence,
                    sapPlaning,
                    equipment,
                    statusList,
                    oemSeverity,
                    tilFocus
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("getTilsReport")]
        public async Task<IActionResult> GetTilPackageReport(EUFilterList reg)
        {
            try
            {
                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());

                List<int> SitesIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.siteList))
                    SitesIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());


                List<int> equipmentIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.equipmentList))
                    equipmentIds = (reg.equipmentList.Split(',').Select(Int32.Parse).ToList());

                List<int> sapIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.sapList))
                    sapIds = (reg.sapList.Split(',').Select(Int32.Parse).ToList());


                List<int> statusIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.statusList))
                    statusIds = (reg.statusList.Split(',').Select(Int32.Parse).ToList());

                List<string> daysIds = new List<string>();
                if (!string.IsNullOrEmpty(reg.daysList))
                    daysIds = (reg.daysList.Split(',').ToList());


                List<int> FocusIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.focusList))
                    FocusIds = (reg.focusList.Split(',').Select(Int32.Parse).ToList());

                List<int> SeveruityIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.severityList))
                    SeveruityIds = (reg.severityList.Split(',').Select(Int32.Parse).ToList());

                List<int> PriorityIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.priorityList))
                    PriorityIds = (reg.priorityList.Split(',').Select(Int32.Parse).ToList());

                var action = await (from tap in _context.TILActionPackage.Where(a => a.isDeleted == 0 && ((PriorityIds.Count == 0) || PriorityIds.Contains((int)a.priorityId)))
                                    join acg in _context.ActionClosureGuidelines on tap.actionClosureGuidelinesId equals acg.acId into allzz2
                                    from bb in allzz2.DefaultIfEmpty()
                                    join outage in _context.OutageTypes on tap.outageId equals outage.outageTypeId into allzz3
                                    from cc in allzz3.DefaultIfEmpty()
                                    join tapeq in _context.TAPEquipment on tap.packageId equals tapeq.tapId
                                    join az in _context.TILActionTracker on new { tap.packageId, tapeq.eqId } equals new { packageId = az.tapId, eqId = az.siteEquipmentId } into allz
                                    from a in allz.DefaultIfEmpty()
                                    join tils in _context.TILBulletin.Where(a => (a.isDeleted == 0) && ((FocusIds.Count == 0) || FocusIds.Contains((int)a.focusId)) && ((SeveruityIds.Count == 0) || SeveruityIds.Contains((int)a.oemSeverityId))) on tap.tilId equals tils.tilId
                                    join f in _context.TILFocus on tils.focusId equals f.focusId into all42
                                    from dd in all42.DefaultIfEmpty()
                                    join os in _context.TILOEMSeverity on tils.oemSeverityId equals os.oemSeverityId into all52
                                    from ee in all52.DefaultIfEmpty()
                                    join se in _context.SiteEquipment.Where(a => (equipmentIds.Count == 0) || equipmentIds.Contains((int)a.equipmentId)) on tapeq.eqId equals se.equipmentId
                                    join s in _context.Sites.Where(a => (SitesIds.Count == 0) || SitesIds.Contains((int)a.siteId)) on se.siteId equals s.siteId
                                    join rege in _context.Regions.Where(a => (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId)) on s.regionId equals rege.regionId
                                    join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                    join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                    join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                    join bs in _context.TAPBudgetSource on tap.budgetSourceId equals bs.budgetSourceId into all
                                    from _bs in all.DefaultIfEmpty()
                                    join df in _context.TAStatus on a.statusCalculated equals df.tasId into asdas
                                    from _sts in asdas.DefaultIfEmpty()
                                    join b in _context.TABudget on a.budgetId equals b.budgetId into all2
                                    from _b in all2.DefaultIfEmpty()
                                    join ps in _context.TAParts on a.partsServiceId equals ps.partId into all3
                                    from _ps in all3.DefaultIfEmpty()
                                    join fi in _context.TAFinalImplementation on a.finalImplementationId equals fi.finalImpId into all4
                                    from _fi in all4.DefaultIfEmpty()
                                    join sap in _context.TASapPlanning on a.sapPlanningId equals sap.sapPlanningId into all5
                                    from _sap in all5.DefaultIfEmpty()
                                    join p in _context.TAPPriority on tap.priorityId equals p.priorityId into all6
                                    from _p in all6.DefaultIfEmpty()
                                    join ev in _context.TAEvidence on a.evidenceId equals ev.evidenceId into all7
                                    from _ev in all7.DefaultIfEmpty()
                                    select new
                                    {
                                        tilNumber = tils.tilNumber,
                                        tilTitle = tils.tilTitle,
                                        tap.packageId,
                                        tapTitle = tap.actionTitle,
                                        tapDescription = tap.actionDescription,
                                        actionDescription = tap.actionDescription,
                                        statusId = a.statusCalculated == null ? 2 : a.statusCalculated,
                                        statustitle = _sts.title == null ? "Open" : _sts.title,
                                        tilActionTrackerId = a.tilActionTrackerId == null ? -1 : a.tilActionTrackerId,
                                        tapId = tap.packageId,
                                        tilAction = tap.actionTitle,
                                        siteEquipmentId = tapeq.eqId,
                                        siteEquipmentTitle = se.unit,
                                        tilDescription = tils.tilTitle,
                                        budgetSourceId = tap.budgetSourceId,
                                        budgetSourceTitle = _bs.budgetSourceTitle,
                                        a.statusCalculated,
                                        a.siteStatusDetail,
                                        a.budgetId,
                                        tils.focusId,
                                        dd.focusTitle,
                                        tils.oemSeverityId,
                                        ee.oemSeverityTitle,
                                        budgetTitle = _b.budgetName,
                                        partServiceId = a.partsServiceId,
                                        partServiceTitle = _ps.partTitle,
                                        planningId = a.sapPlanningId== null?-1: a.sapPlanningId,
                                        planningTitle = _sap.sapPlanningTitle,
                                        finalImplementationTitle =
                                        _fi.finalImpTitle,
                                        finalImpScore = _fi.score == null ? 0 : _fi.score,
                                        finalImplementationId = a.finalImplementationId,
                                        targetDate = a.targetDate == null ? tap.createdOn.AddDays(30) : a.targetDate,
                                        priorityId = tap.priorityId,
                                        priorityTitle = _p.priorityTitle,
                                        a.calStatus,
                                        a.calcPriority,
                                        a.budgetCalc,
                                        a.ddtCalc,
                                        a.evidenceCalc,
                                        a.implementationCalc,
                                        a.partsCalc,
                                        a.sapCalc,
                                        rege.regionId,
                                        regionTitle = rege.title,
                                        s.siteId,
                                        siteTitle = s.siteName,
                                        a.evidenceId,
                                        tap.actionClosureGuidelinesId,
                                        actionCategory = bb.title,
                                        tap.outageId,
                                        unitStatus = cc.title,
                                        evidenceTitle = _ev.evidenceTitle,
                                    }).Distinct().ToListAsync();

                var actionComplete = action
                    //.Where(a => ((statusIds.Count == 0) || statusIds.Contains((int)a.statusId)) && ((sapIds.Count == 0) || sapIds.Contains((int)a.planningId)))
                                   //&& (a.targetDate >= Convert.ToDateTime(reg.filter.startDate) || reg.filter.startDate == null) && (a.targetDate <= Convert.ToDateTime(reg.filter.endDate) || reg.filter.endDate == null))
                                   .Select(a => new {
                    
                    a.packageId,
                    a.tapTitle,
                    a.tapDescription,
                    a.actionDescription,
                                       statusId = DaysToTargetCalculation.CalculateStatusId(a.finalImpScore),
                                       statustitle = DaysToTargetCalculation.CalculateStatusTitle(a.finalImpScore),
                                       a.tilActionTrackerId,
                    a.tapId,
                    a.tilAction,
                    a.siteEquipmentId,
                    a.siteEquipmentTitle,
                    a.tilDescription,
                    a.budgetSourceId,
                    a.budgetSourceTitle,
                    a.statusCalculated,
                    a.siteStatusDetail,
                    a.budgetId,
                    a.budgetTitle,
                    a.partServiceId,
                                       a.focusId,
                                       a.focusTitle,
                                       a.oemSeverityId,
                                       a.oemSeverityTitle,
                                       a.partServiceTitle,
                    a.planningId,
                    a.tilNumber,
                    a.planningTitle,
                    a.finalImplementationTitle,
                    a.finalImplementationId,
                    a.targetDate,
                    a.priorityId,
                    a.priorityTitle,
                    a.calStatus,
                    a.calcPriority,
                    a.budgetCalc,
                    a.ddtCalc,
                    a.evidenceCalc,
                    a.implementationCalc,
                    a.partsCalc,
                    a.sapCalc,
                    a.regionId,
                    a.regionTitle,
                    a.siteId,
                    a.siteTitle,
                    a.evidenceId,
                    a.actionClosureGuidelinesId,
                    a.actionCategory,
                    a.outageId,
                    a.unitStatus,
                    a.evidenceTitle,
                    daysToTarget = DaysToTargetCalculation.NewCalcTil(a.finalImpScore, a.targetDate),
                                   
                                   }).ToList();
                var obj = new
                {
                    action = actionComplete.Where(a => ((daysIds.Count == 0) || daysIds.Contains((string)a.daysToTarget)) && ((statusIds.Count == 0) || statusIds.Contains((int)a.statusId)) && ((sapIds.Count == 0) || sapIds.Contains((int)a.planningId))).ToList(),
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("getTils")]
        public async Task<IActionResult> GetTilPackage(EUFilterList reg)
        {
            try
            {
                
                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());

                List<int> SitesIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.siteList))
                    SitesIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());


                List<int> equipmentIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.equipmentList))
                    equipmentIds = (reg.equipmentList.Split(',').Select(Int32.Parse).ToList());

                List<int> sapIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.sapList))
                    sapIds = (reg.sapList.Split(',').Select(Int32.Parse).ToList());
                
                
                List<int> statusIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.statusList))
                    statusIds = (reg.statusList.Split(',').Select(Int32.Parse).ToList());

                List<string> daysIds = new List<string>();
                if (!string.IsNullOrEmpty(reg.daysList))
                    daysIds = (reg.daysList.Split(',').ToList());

                List<int> FocusIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.focusList))
                    FocusIds = (reg.focusList.Split(',').Select(Int32.Parse).ToList());

                List<int> SeveruityIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.severityList))
                    SeveruityIds = (reg.severityList.Split(',').Select(Int32.Parse).ToList());

                List<int> PriorityIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.priorityList))
                    PriorityIds = (reg.priorityList.Split(',').Select(Int32.Parse).ToList());

                var action = await (from tap in _context.TILActionPackage.Where(a => a.isDeleted == 0 && ((PriorityIds.Count == 0) || PriorityIds.Contains((int)a.priorityId)))
                                    join acg in _context.ActionClosureGuidelines on tap.actionClosureGuidelinesId equals acg.acId into allzz2
                                    from bb in allzz2.DefaultIfEmpty()
                                    join outage in _context.OutageTypes on tap.outageId equals outage.outageTypeId into allzz3
                                    from cc in allzz3.DefaultIfEmpty()
                                    join tapeq in _context.TAPEquipment on tap.packageId equals tapeq.tapId
                                    join az in _context.TILActionTracker on new { tap.packageId, tapeq.eqId } equals new { packageId = az.tapId, eqId = az.siteEquipmentId } into allz
                                    from a in allz.DefaultIfEmpty()
                                    join tils in _context.TILBulletin.Where(a=>(a.isDeleted == 0)&&((FocusIds.Count == 0) || FocusIds.Contains((int)a.focusId)) && ((SeveruityIds.Count == 0) || SeveruityIds.Contains((int)a.oemSeverityId))) on tap.tilId equals tils.tilId
                                    join f in _context.TILFocus on tils.focusId equals f.focusId into all42
                                    from dd in all42.DefaultIfEmpty()
                                    join os in _context.TILOEMSeverity on tils.oemSeverityId equals os.oemSeverityId into all52
                                    from ee in all52.DefaultIfEmpty()
                                    join se in _context.SiteEquipment.Where(a => (equipmentIds.Count == 0) || equipmentIds.Contains((int)a.equipmentId)) on tapeq.eqId equals se.equipmentId
                                    join s in _context.Sites.Where(a => (SitesIds.Count == 0) || SitesIds.Contains((int)a.siteId)) on se.siteId equals s.siteId
                                    join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                    join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                    join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                    join cn in _context.Country on s.countryId equals cn.countryId
                                    join cnvp in _context.CountryExecutiveVp on cn.countryId equals cnvp.countryId into allcn
                                    from cnvpp in allcn.DefaultIfEmpty()
                                    join rege in _context.Regions.Where(a => (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId)) on s.regionId equals rege.regionId
                                    join rvp in _context.RegionsExecutiveVp on rege.regionId equals rvp.regionId into allrege
                                    from rvpp in allrege.DefaultIfEmpty()
                                    join bs in _context.TAPBudgetSource on tap.budgetSourceId equals bs.budgetSourceId into all
                                    from _bs in all.DefaultIfEmpty()
                                    join df in _context.TAStatus on a.statusCalculated equals df.tasId into asdas
                                    from _sts in asdas.DefaultIfEmpty()
                                    join b in _context.TABudget on a.budgetId equals b.budgetId into all2
                                    from _b in all2.DefaultIfEmpty()
                                    join ps in _context.TAParts on a.partsServiceId equals ps.partId into all3
                                    from _ps in all3.DefaultIfEmpty()
                                    join fi in _context.TAFinalImplementation on a.finalImplementationId equals fi.finalImpId into all4
                                    from _fi in all4.DefaultIfEmpty()
                                    join sap in _context.TASapPlanning on a.sapPlanningId equals sap.sapPlanningId into all5
                                    from _sap in all5.DefaultIfEmpty()
                                    join p in _context.TAPPriority on tap.priorityId equals p.priorityId into all6
                                    from _p in all6.DefaultIfEmpty()
                                    join ev in _context.TAEvidence on a.evidenceId equals ev.evidenceId into all7
                                    from _ev in all7.DefaultIfEmpty()
                                    join au in _context.AppUser on a.assignedTo equals au.userId into all8
                                    from auu in all8.DefaultIfEmpty()
                                    where (s.tilPocId == reg.userId || rege.executiveDirector == reg.userId || rvpp.userId == reg.userId ||cnvpp.userId == reg.userId || cn.executiveDirector == reg.userId ||
                                    (a == null ? -1 : a.assignedTo) == reg.userId || reg.userId == 22 || reg.userId == 1)
                                    select new
                                    {
                                        assignedToId = a.assignedTo,
                                        assignedToTitle = auu.userName,
                                        tap.packageId,
                                        tapTitle = tap.actionTitle,
                                        tilNumber = tils.tilNumber,
                                        tapDescription = tap.actionDescription,
                                        actionDescription = tap.actionDescription,
                                        statusId = a.statusCalculated == null ? 2 : a.statusCalculated,
                                        statustitle = _sts.title == null ? "Open" : _sts.title,
                                        tilActionTrackerId = a.tilActionTrackerId == null ? -1 : a.tilActionTrackerId,
                                        tapId = tap.packageId,
                                        tilAction = tap.actionTitle,
                                        siteEquipmentId = tapeq.eqId,
                                        siteEquipmentTitle = se.unit,
                                        tilDescription = tils.tilTitle,
                                        budgetSourceId = tap.budgetSourceId,
                                        budgetSourceTitle = _bs.budgetSourceTitle,
                                        a.statusCalculated,
                                        a.siteStatusDetail,
                                        a.budgetId,
                                        budgetTitle = _b.budgetName,
                                        partServiceId = a.partsServiceId,
                                        partServiceTitle = _ps.partTitle,
                                        planningId = a.sapPlanningId,
                                        planningTitle = _sap.sapPlanningTitle,
                                        finalImplementationTitle =
                                        _fi.finalImpTitle,
                                        finalImpScore = _fi.score == null?0: _fi.score,
                                        tils.focusId,
                                        dd.focusTitle,
                                        tils.oemSeverityId,
                                        ee.oemSeverityTitle,
                                        finalImplementationId = a.finalImplementationId,
                                        targetDate = a.targetDate == null ? tap.createdOn.AddDays(30) : a.targetDate,
                                        priorityId = tap.priorityId,
                                        priorityTitle = _p.priorityTitle,
                                        a.calStatus,
                                        a.calcPriority,
                                        a.budgetCalc,
                                        a.ddtCalc,
                                        a.evidenceCalc,
                                        a.implementationCalc,
                                        a.partsCalc,
                                        a.sapCalc,
                                        s.siteId,
                                        rege.regionId,
                                        regionTitle = rege.title,
                                        siteTitle = s.siteName,
                                        a.evidenceId,
                                        tap.actionClosureGuidelinesId,
                                        actionCategory = bb.title,
                                        tap.outageId,
                                        unitStatus = cc.title,
                                        evidenceTitle = _ev.evidenceTitle,
                                    }).Distinct().ToListAsync();
                var actionComplete = action
                    //.Where(a => ((statusIds.Count == 0) || statusIds.Contains((int)a.statusId)) && ((sapIds.Count == 0) || sapIds.Contains((int)a.planningId)))
                //&& (a.targetDate >= Convert.ToDateTime(reg.filter.startDate) || reg.filter.startDate == null) && (a.targetDate <= Convert.ToDateTime(reg.filter.endDate) || reg.filter.endDate == null))
                    .Select(a => new
                    {
                        a.assignedToId,
                        a.assignedToTitle,
                        a.packageId,
                        a.tapTitle,
                        a.tapDescription,
                        a.actionDescription,
                        a.finalImpScore,
                        statusId= DaysToTargetCalculation.CalculateStatusId(a.finalImpScore),
                        statustitle= DaysToTargetCalculation.CalculateStatusTitle(a.finalImpScore),
                        a.tilActionTrackerId,
                        a.tapId,
                        a.tilAction,
                        a.siteEquipmentId,
                        a.siteEquipmentTitle,
                        a.tilDescription,
                        a.budgetSourceId,
                        a.budgetSourceTitle,
                        a.statusCalculated,
                        a.siteStatusDetail,
                        a.budgetId,
                        a.tilNumber,
                        a.regionId,
                        a.regionTitle,
                        a.budgetTitle,
                        a.partServiceId,
                        a.partServiceTitle,
                        a.planningId,
                        a.planningTitle,
                        a.finalImplementationTitle,
                        a.finalImplementationId,
                        a.targetDate,
                        a.priorityId,
                        a.priorityTitle,
                        a.calStatus,
                        a.calcPriority,
                        a.budgetCalc,
                        a.ddtCalc,
                        a.evidenceCalc,
                        a.implementationCalc,
                        a.partsCalc,
                        a.sapCalc,
                        a.siteId,
                        a.siteTitle,
                        a.evidenceId,
                        a.actionClosureGuidelinesId,
                        a.actionCategory,
                        a.outageId,
                        a.unitStatus,
                        a.focusId,
                        a.focusTitle,
                        a.oemSeverityId,
                        a.oemSeverityTitle,
                        a.evidenceTitle,
                        daysToTarget = DaysToTargetCalculation.NewCalcTil(a.finalImpScore, a.targetDate),
                    }).ToList();
                var obj = new
                {
                    action = actionComplete.Where(a => ((daysIds.Count == 0) || daysIds.Contains((string)a.daysToTarget)) && ((statusIds.Count == 0) || statusIds.Contains((int)a.statusId)) && ((sapIds.Count == 0) || sapIds.Contains((int)a.planningId))).ToList(),
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("saveTils")]
        public async Task<IActionResult> UpdateActionTracker(tilPackagetrackeruser reg)
        {
            try
            {
                if (reg.action.tilActionTrackerId != -1)
                {
                    TILActionTracker til = (from t in _context.TILActionTracker.Where(a => a.tilActionTrackerId == reg.action.tilActionTrackerId)
                                            select t).FirstOrDefault();
                    til.siteStatusDetail = reg.action.siteStatusDetail;
                    til.partsServiceId = reg.action.partServiceId;
                    til.tapId = (int)reg.action.tapId;
                    til.finalImplementationId = reg.action.finalImplementationId;
                    til.evidenceId = reg.action.evidenceId;
                    til.sapPlanningId = reg.action.planningId;
                    til.targetDate = reg.action.targetDate;
                    til.budgetId = reg.action.budgetId;
                    til.siteEquipmentId = (int)reg.action.siteEquipmentId;
                    if (reg.action.assignedToId != null)
                    {
                        til.assignedTo = reg.action.assignedToId;
                    }

                    TABudget budgetScore = new TABudget();
                    TAFinalImplementation implmentationScore = new TAFinalImplementation();
                    TAParts partScore = new TAParts();
                    TASapPlanning sapScore = new TASapPlanning();
                    TAPPriority priorityScore = new TAPPriority();
                    TAEvidence evidenceScore = new TAEvidence();
                    if (reg.action.budgetId != null)
                    {
                        budgetScore = (from b in _context.TABudget.Where(a => a.budgetId == reg.action.budgetId)
                                       select b).FirstOrDefault();
                        til.budgetCalc = (budgetScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.budgetCalc = "0%";
                    }
                    if (reg.action.finalImplementationId != null)
                    {
                        implmentationScore = (from b in _context.TAFinalImplementation.Where(a => a.finalImpId == reg.action.finalImplementationId)
                                              select b).FirstOrDefault();
                        til.implementationCalc = (implmentationScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.implementationCalc = "0%";
                    }


                    if (reg.action.partServiceId != null)
                    {
                        partScore = (from b in _context.TAParts.Where(a => a.partId == reg.action.partServiceId)
                                     select b).FirstOrDefault();
                        til.partsCalc = (partScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.partsCalc = "0%";
                    }

                    if (reg.action.planningId != null)
                    {
                        sapScore = (from b in _context.TASapPlanning.Where(a => a.sapPlanningId == reg.action.planningId)
                                    select b).FirstOrDefault();
                        til.sapCalc = (sapScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.sapCalc = "0%";
                    }
                    if (reg.action.priorityId != null)
                    {
                        priorityScore = (from b in _context.TAPPriority.Where(a => a.priorityId == reg.action.priorityId)
                                         select b).FirstOrDefault();
                        til.calcPriority = (priorityScore.score * 100);
                    }
                    else
                    {
                        til.calcPriority = 0;
                    }
                    if (reg.action.evidenceId != null)
                    {
                        evidenceScore = (from b in _context.TAEvidence.Where(a => a.evidenceId == reg.action.evidenceId)
                                         select b).FirstOrDefault();
                        til.evidenceCalc = (evidenceScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.evidenceCalc = "0%";

                    }


                    til.calStatus = (((budgetScore.score + implmentationScore.score + partScore.score + sapScore.score + evidenceScore.score) / 5) * 100).ToString() + "%";

                    if (implmentationScore.score == 1)
                    {
                        til.statusCalculated = 3;
                    }
                    else if (implmentationScore.score == 0 || implmentationScore.score == null)
                    {
                        til.statusCalculated = 2;
                    }

                    else
                    {
                        til.statusCalculated = 4;
                    }

                    til.modifiedOn = DateTime.Now;
                    til.modifiedBy = reg.userId;
                    _context.SaveChanges();


                }
                else
                {
                    TILActionTracker til = new TILActionTracker();
                    til.siteStatusDetail = reg.action.siteStatusDetail;
                    til.partsServiceId = reg.action.partServiceId;
                    til.tapId = (int)reg.action.tapId;
                    //til.budgetSourceId = reg.action.budgetSourceId;
                    til.finalImplementationId = reg.action.finalImplementationId;
                    til.evidenceId = reg.action.evidenceId;
                    til.sapPlanningId = reg.action.planningId;
                    til.targetDate = reg.action.targetDate;
                    til.budgetId = reg.action.budgetId;
                    til.siteEquipmentId = (int)reg.action.siteEquipmentId;
                    if (reg.action.assignedToId != null)
                    {
                        til.assignedTo = reg.action.assignedToId;
                    }
                    //if (reg.action.planningId == 1 || reg.action.planningId == 11)
                    //{
                    //    reg.action.statusCalculated = 2;
                    //}
                    //else if (reg.action.planningId == 3 || reg.action.planningId == 4 || reg.action.planningId == 5 || reg.action.planningId == 6 || reg.action.planningId == 9 || reg.action.planningId == 10)
                    //{
                    //    reg.action.statusCalculated = 3;
                    //}
                    //else if (reg.action.planningId == 1 || reg.action.planningId == 7 || reg.action.planningId == 8)
                    //{
                    //    reg.action.statusCalculated = 4;
                    //}
                    //else
                    //{
                    //    reg.action.statusCalculated = 4;
                    //}

                    //til.statusCalculated = (int)reg.action.statusCalculated;
                    //var budgetScore = (from b in _context.TABudget.Where(a => a.budgetId == reg.action.budgetId)
                    //                   select b).FirstOrDefault();
                    //til.budgetCalc = (budgetScore.score * 100).ToString() + "%";

                    //var statusScore = (from b in _context.TAStatus.Where(a => a.tasId == reg.action.statusCalculated)
                    //                   select b).FirstOrDefault();
                    //til.calStatus = (statusScore.score * 100).ToString() + "%";

                    //var evidenceCalc = (from b in _context.TAEvidence.Where(a => a.evidenceId == reg.action.evidenceId)
                    //                    select b).FirstOrDefault();
                    //til.evidenceCalc = (evidenceCalc.score * 100).ToString() + "%";
                    //var implmentationScore = (from b in _context.TAFinalImplementation.Where(a => a.finalImpId == reg.action.finalImplementationId)
                    //                          select b).FirstOrDefault();
                    //til.implementationCalc = (implmentationScore.score * 100).ToString() + "%";
                    //var partScore = (from b in _context.TAParts.Where(a => a.partId == reg.action.partServiceId)
                    //                 select b).FirstOrDefault();
                    //til.partsCalc = (partScore.score * 100).ToString() + "%";
                    //var sapScore = (from b in _context.TASapPlanning.Where(a => a.sapPlanningId == reg.action.planningId)
                    //                select b).FirstOrDefault();
                    //til.partsCalc = (sapScore.score * 100).ToString() + "%";

                    //if (reg.action.statusCalculated == 3)
                    //{
                    //    var ddt = (from a in _context.TADayToTarget.Where(a => a.dayId == 5)
                    //               select a).FirstOrDefault();
                    //    til.ddtCalc = (ddt.score * 100).ToString() + "%";
                    //}
                    TABudget budgetScore = new TABudget();
                    TAFinalImplementation implmentationScore = new TAFinalImplementation();
                    TAParts partScore = new TAParts();
                    TASapPlanning sapScore = new TASapPlanning();
                    TAPPriority priorityScore = new TAPPriority();
                    TAEvidence evidenceScore = new TAEvidence();
                    if (reg.action.budgetId != null)
                    {
                        budgetScore = (from b in _context.TABudget.Where(a => a.budgetId == reg.action.budgetId)
                                           select b).FirstOrDefault();
                        til.budgetCalc = (budgetScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.budgetCalc = "0%";
                    }
                    if(reg.action.finalImplementationId != null)
                    {
                        implmentationScore = (from b in _context.TAFinalImplementation.Where(a => a.finalImpId == reg.action.finalImplementationId)
                                                  select b).FirstOrDefault();
                        til.implementationCalc = (implmentationScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.implementationCalc = "0%";
                    }
                   

                   if(reg.action.partServiceId != null)
                    {
                        partScore = (from b in _context.TAParts.Where(a => a.partId == reg.action.partServiceId)
                                         select b).FirstOrDefault();
                        til.partsCalc = (partScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.partsCalc = "0%";
                    }

                    if (reg.action.planningId != null)
                    {
                        sapScore = (from b in _context.TASapPlanning.Where(a => a.sapPlanningId == reg.action.planningId)
                                        select b).FirstOrDefault();
                        til.sapCalc = (sapScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.sapCalc = "0%";
                    }
                    if(reg.action.priorityId != null)
                    {
                        priorityScore = (from b in _context.TAPPriority.Where(a => a.priorityId == reg.action.priorityId)
                                             select b).FirstOrDefault();
                        til.calcPriority = (priorityScore.score * 100);
                    }
                    else
                    {
                        til.calcPriority = 0;
                    }
                    if (reg.action.evidenceId != null)
                    {
                        evidenceScore = (from b in _context.TAEvidence.Where(a => a.evidenceId == reg.action.evidenceId)
                                             select b).FirstOrDefault();
                        til.evidenceCalc = (evidenceScore.score * 100).ToString() + "%";
                    }
                    else
                    {
                        til.evidenceCalc = "0%";

                    }

                    til.calStatus = (((budgetScore.score + implmentationScore.score + partScore.score + sapScore.score + evidenceScore.score) / 5) * 100).ToString() + "%";
                    //if (budgetScore.score == 1 && implmentationScore.score == 1 && partScore.score == 1 && sapScore.score == 1)
                    //{
                    //    til.statusCalculated = 3;
                    //}
                    //else if (budgetScore.score == 0 && implmentationScore.score == 0 && partScore.score == 0 && sapScore.score == 0)
                    //{
                    //    til.statusCalculated = 2;
                    //}
                    if (implmentationScore.score == 1)
                    {
                        til.statusCalculated = 3;
                    }
                    else if (implmentationScore.score == 0 || implmentationScore.score == null)
                    {
                        til.statusCalculated = 2;
                    }
                  
                    else
                    {
                        til.statusCalculated = 4;
                    }


                    //TILActiontrackerFile f = await (from a in _context.TILActiontrackerFile.Where(a => a.tapId == reg.action.tapId && a.equipId == reg.action.siteEquipmentId)
                    //                                select a).FirstOrDefaultAsync();
                    //if (f != null)
                    //{
                    //    var evidenceScore = (from b in _context.TAEvidence.Where(a => a.evidenceId == 2)
                    //                         select b).FirstOrDefault();
                    //    til.evidenceCalc = (evidenceScore.score * 100).ToString() + "%";
                    //};
                    //if (f == null)
                    //{
                    //    var evidenceScore = (from b in _context.TAEvidence.Where(a => a.evidenceId == 1)
                    //                         select b).FirstOrDefault();
                    //    til.evidenceCalc = (evidenceScore.score * 100).ToString() + "%";
                    //};

                    til.createdOn = DateTime.Now;
                    til.createdBy = reg.userId;
                    _context.Add(til);
                    _context.SaveChanges();
                }
                return Ok(reg.action);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
