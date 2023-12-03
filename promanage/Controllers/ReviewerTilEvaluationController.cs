using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ActionTrakingSystem.DTOs;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ActionTrakingSystem.Controllers
{

    public class ReviewerTilEvaluationController : BaseAPIController
    {
        private readonly DAL _context;
        public ReviewerTilEvaluationController(DAL context)
        {
            _context = context;
        }
        [HttpPost("getTils")]
        public async Task<IActionResult> GetTils(getTilUserFilterDto rege)
        {
            try
            {
                List<int> DocIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.docTypeList))
                    DocIds = (rege.docTypeList.Split(',').Select(Int32.Parse).ToList());

                List<int> StausIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.statusList))
                    StausIds = (rege.statusList.Split(',').Select(Int32.Parse).ToList());


                List<int> FormIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.formList))
                    FormIds = (rege.formList.Split(',').Select(Int32.Parse).ToList());
                var tils = await (from t in _context.TILBulletin.Where(a => a.isDeleted == 0)
                                  join c in _context.TILComponent on t.componentId equals c.componentId into all
                                  from aa in all.DefaultIfEmpty()
                                  join d in _context.TILDocumentType on t.documentTypeId equals d.typeId into all2
                                  from bb in all2.DefaultIfEmpty()
                                  join f in _context.TILFocus on t.focusId equals f.focusId into all4
                                  from dd in all4.DefaultIfEmpty()
                                  join os in _context.TILOEMSeverity on t.oemSeverityId equals os.oemSeverityId into all5
                                  from ee in all5.DefaultIfEmpty()
                                  join ot in _context.TILOEMTimimgCode on t.oemTimimgCodeId equals ot.timingId into all6
                                  from ff in all6.DefaultIfEmpty()
                                  join rf in _context.TILReviewForm on t.reviewForumId equals rf.reviewFormId into all7
                                  from gg in all7.DefaultIfEmpty()
                                  join rs in _context.TILReviewStatus on t.reviewStatusId equals rs.reviewStatusId into all8
                                  from hh in all8.DefaultIfEmpty()
                                  join ts in _context.TILSource on t.sourceId equals ts.sourceId into all9
                                  from ii in all9.DefaultIfEmpty()
                                  join techEval in _context.TechnicalEvaluation on t.technicalReviewId equals techEval.teId into all12
                                  from nn in all12.DefaultIfEmpty()
                                  select new
                                  {
                                      t.tilId,
                                      t.tilNumber,
                                      t.alternateNumber,
                                      t.applicabilityNotes,
                                      t.tilTitle,
                                      t.currentRevision,
                                      tilFocusId = t.focusId,
                                      tilFocusTitle = dd.focusTitle,
                                      documentTypeId = t.documentTypeId,
                                      documentTypeTitle = bb.typeTitle,
                                      t.oem,
                                      oemSeverityId = t.oemSeverityId,
                                      oemSeverityTitle = ee.oemSeverityTitle,
                                      oemTimingId = t.oemTimimgCodeId,
                                      oemTimingTitle = ff.timingCode,
                                      reviewForumId = t.reviewForumId,
                                      reviewForumtitle = gg.reviewFormTitle,
                                      t.recommendations,
                                      technicalReviewId = nn.teId,
                                      technicalReviewUserId = nn.userId,
                                      technicalReviewSummary = nn.technicalEvaluation,
                                      t.dateReceivedNomac,
                                      t.dateIssuedDocument,
                                      sourceId = t.sourceId,
                                      sourceTitle = ii.sourceTitle,
                                      reviewStatusId = t.reviewStatusId,
                                      reviewStatusTitle = hh.reviewStatusTitle,
                                      t.notes,
                                      componentId = t.componentId,
                                      componentTitle = aa.componentTitle,
                                      t.implementationNotes,
                                      t.report,
                                      t.yearOfIssue
                                  }).OrderByDescending(a=>a.tilId).ToListAsync();
                var tilComponent = await (from tc in _context.TILComponent.Where(a => a.isDeleted == 0)
                                          select new
                                          {
                                              tc.componentId,
                                              tc.componentTitle
                                          }).ToListAsync();
                var tilDocType = await (from tc in _context.TILDocumentType.Where(a => a.isDeleted == 0)
                                        select new
                                        {
                                            tc.typeId,
                                            tc.typeTitle
                                        }).ToListAsync();
                var tilEquipment = await (from tc in _context.TILEquipment.Where(a => a.isDeleted == 0)
                                          select new
                                          {
                                              tc.equipmentId,
                                              tc.equipmentTitle
                                          }).ToListAsync();
                var oemSeverity = await (from tc in _context.TILOEMSeverity.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             tc.oemSeverityId,
                                             tc.oemSeverityTitle
                                         }).ToListAsync();
                var oemSeverityTiming = await (from tc in _context.TILOEMTimimgCode.Where(a => a.isDeleted == 0)
                                               select new
                                               {
                                                   tc.timingId,
                                                   tc.timingCode
                                               }).ToListAsync();

                var tilFocus = await (from tc in _context.TILFocus.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          tc.focusId,
                                          tc.focusTitle
                                      }).ToListAsync();
                var reviewForum = await (from tc in _context.TILReviewForm.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             tc.reviewFormId,
                                             tc.reviewFormTitle
                                         }).ToListAsync();
                var reviewStatus = await (from tc in _context.TILReviewStatus.Where(a => a.isDeleted == 0)
                                          select new
                                          {
                                              tc.reviewStatusId,
                                              tc.reviewStatusTitle
                                          }).ToListAsync();
                var tilSite = await (from tc in _context.Sites.Where(a => a.isDeleted == 0)
                                     select new
                                     {
                                         tc.siteId,
                                         tc.siteName
                                     }).ToListAsync();
                var tilSource = await (from tc in _context.TILSource.Where(a => a.isDeleted == 0)
                                       select new
                                       {
                                           tc.sourceId,
                                           tc.sourceTitle
                                       }).ToListAsync();
                var obj = new
                {
                    tils,
                    tilComponent,
                    tilDocType,
                    tilEquipment,
                    oemSeverity,
                    oemSeverityTiming,
                    tilFocus,
                    reviewForum,
                    reviewStatus,
                    tilSite,
                    tilSource,
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Authorize]
        [HttpPost("saveReview")]
        public async Task<IActionResult> SaveReview(ReviewerTilEvaluationDto reg)
        {
            try
            {
                if (reg.til.technicalReviewId == -1)
                {
                    TechnicalEvaluation te = new TechnicalEvaluation();
                    te.technicalEvaluation = reg.til.technicalReviewSummary;
                    te.createdDate = DateTime.Now;
                    te.userId = reg.userId;
                    te.createdBy = reg.userId;
                    te.isDeleted = 0;
                    _context.Add(te);
                    _context.SaveChanges();
                    TILBulletin til = await (from r in _context.TILBulletin.Where(a => a.tilId == reg.til.tilId)
                                             select r).FirstOrDefaultAsync();
                    til.technicalReviewId = te.teId;
                    _context.SaveChanges();
                    reg.til.technicalReviewId = te.teId;
                    return Ok(reg.til);
                }
                else
                {
                    TechnicalEvaluation tee = await (from r in _context.TechnicalEvaluation.Where(a => a.teId == reg.til.technicalReviewId)
                                                     select r).FirstOrDefaultAsync();
                    tee.technicalEvaluation = reg.til.technicalReviewSummary;
                    _context.SaveChanges();
                    return Ok(reg.til);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
