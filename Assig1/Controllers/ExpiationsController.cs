using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assig1.Data;
using Assig1.Models;
using Assig1.ViewModels;

namespace Assig1.Controllers
{
    public class ExpiationsController : Controller
    {
        private readonly ExpiationsContext _context;

        public ExpiationsController(ExpiationsContext context)
        {
            _context = context;
        }

        // GET: Expiations

        public async Task<IActionResult> Index(string statusFilter, DateOnly? year, DateOnly? month)
        {
            var expiationsQuery = _context.Expiations.AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                expiationsQuery = expiationsQuery.Where(e => e.StatusCode == statusFilter);
            }
            if (year.HasValue)
            {
                expiationsQuery = expiationsQuery.Where(e => e.IssueDate.HasValue && e.IssueDate.Value.Year == year.Value.Year);
            }

            if (month.HasValue)
            {
                expiationsQuery = expiationsQuery.Where(e => e.IssueDate.HasValue && e.IssueDate.Value.Month == month.Value.Month);
            }

            var expiations = await expiationsQuery.ToListAsync();

            // Calculate statistics for fine amounts and speeds
            var maxFine = expiations.Max(e => e.TotalFeeAmt ?? 0);
            var minFine = expiations.Min(e => e.TotalFeeAmt ?? 0);
            var avgFine = expiations.Average(e => e.TotalFeeAmt ?? 0);

            var maxSpeed = expiations.Where(e => e.VehicleSpeed.HasValue).Max(e => e.VehicleSpeed.Value);
            var avgSpeed = expiations.Where(e => e.VehicleSpeed.HasValue).Average(e => e.VehicleSpeed.Value);

            // Calculate the frequency of expiations
            var frequency = expiations.Count;

            // Create the ViewModel
            var viewModel = new ExpiationInsightsViewModel
            {
                StatusFilter = statusFilter,
                Year = year,
                Month = month,
                MaxFine = maxFine,
                MinFine = minFine,
                AvgFine = avgFine,
                MaxSpeed = maxSpeed,
                AvgSpeed = avgSpeed,
                Frequency = frequency
            };

            return View(viewModel);
        }

        // GET: Expiations/Details/5
        public async Task<IActionResult> Details(DateOnly? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expiation = await _context.Expiations
                .Include(e => e.CameraCode)
                .Include(e => e.DriverStateNavigation)
                .Include(e => e.LsaCodeNavigation)
                .Include(e => e.OffenceCodeNavigation)
                .Include(e => e.PhotoRejCodeNavigation)
                .Include(e => e.RegStateNavigation)
                .Include(e => e.StatusCodeNavigation)
                .Include(e => e.TypeCodeNavigation)
                .Include(e => e.WithdrawCodeNavigation)
                .FirstOrDefaultAsync(m => m.IncidentStartDate == id);
            if (expiation == null)
            {
                return NotFound();
            }

            return View(expiation);
        }

        // GET: Expiations/Create
        public IActionResult Create()
        {
            ViewData["CameraLocationId"] = new SelectList(_context.CameraCodes, "LocationId", "CameraTypeCode");
            ViewData["DriverState"] = new SelectList(_context.CountryStates, "CountryStateCode", "CountryStateCode");
            ViewData["LsaCode"] = new SelectList(_context.LocalServiceAreas, "LsaCode", "LsaCode");
            ViewData["OffenceCode"] = new SelectList(_context.Offences, "OffenceCode", "OffenceCode");
            ViewData["PhotoRejCode"] = new SelectList(_context.PhotoRejections, "RejectionCode", "RejectionCode");
            ViewData["RegState"] = new SelectList(_context.CountryStates, "CountryStateCode", "CountryStateCode");
            ViewData["StatusCode"] = new SelectList(_context.NoticeStates, "StatusCode", "StatusCode");
            ViewData["TypeCode"] = new SelectList(_context.NoticeTypes, "TypeCode", "TypeCode");
            ViewData["WithdrawCode"] = new SelectList(_context.WithdrawnReasons, "WithdrawCode", "WithdrawCode");
            return View();
        }

        // POST: Expiations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpId,IncidentStartDate,IncidentStartTime,IssueDate,OffencePenaltyAmt,OffenceLevyAmt,CorporateFeeAmt,TotalFeeAmt,EnforceWarningNoticeFeeAmt,BacContentExp,VehicleSpeed,LocationSpeedLimit,OffenceCode,DriverState,RegState,LsaCode,CameraLocationId,CameraTypeCode,PhotoRejCode,StatusCode,WithdrawCode,TypeCode")] Expiation expiation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expiation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CameraLocationId"] = new SelectList(_context.CameraCodes, "LocationId", "CameraTypeCode", expiation.CameraLocationId);
            ViewData["DriverState"] = new SelectList(_context.CountryStates, "CountryStateCode", "CountryStateCode", expiation.DriverState);
            ViewData["LsaCode"] = new SelectList(_context.LocalServiceAreas, "LsaCode", "LsaCode", expiation.LsaCode);
            ViewData["OffenceCode"] = new SelectList(_context.Offences, "OffenceCode", "OffenceCode", expiation.OffenceCode);
            ViewData["PhotoRejCode"] = new SelectList(_context.PhotoRejections, "RejectionCode", "RejectionCode", expiation.PhotoRejCode);
            ViewData["RegState"] = new SelectList(_context.CountryStates, "CountryStateCode", "CountryStateCode", expiation.RegState);
            ViewData["StatusCode"] = new SelectList(_context.NoticeStates, "StatusCode", "StatusCode", expiation.StatusCode);
            ViewData["TypeCode"] = new SelectList(_context.NoticeTypes, "TypeCode", "TypeCode", expiation.TypeCode);
            ViewData["WithdrawCode"] = new SelectList(_context.WithdrawnReasons, "WithdrawCode", "WithdrawCode", expiation.WithdrawCode);
            return View(expiation);
        }

        // GET: Expiations/Edit/5
        public async Task<IActionResult> Edit(DateOnly? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expiation = await _context.Expiations.FindAsync(id);
            if (expiation == null)
            {
                return NotFound();
            }
            ViewData["CameraLocationId"] = new SelectList(_context.CameraCodes, "LocationId", "CameraTypeCode", expiation.CameraLocationId);
            ViewData["DriverState"] = new SelectList(_context.CountryStates, "CountryStateCode", "CountryStateCode", expiation.DriverState);
            ViewData["LsaCode"] = new SelectList(_context.LocalServiceAreas, "LsaCode", "LsaCode", expiation.LsaCode);
            ViewData["OffenceCode"] = new SelectList(_context.Offences, "OffenceCode", "OffenceCode", expiation.OffenceCode);
            ViewData["PhotoRejCode"] = new SelectList(_context.PhotoRejections, "RejectionCode", "RejectionCode", expiation.PhotoRejCode);
            ViewData["RegState"] = new SelectList(_context.CountryStates, "CountryStateCode", "CountryStateCode", expiation.RegState);
            ViewData["StatusCode"] = new SelectList(_context.NoticeStates, "StatusCode", "StatusCode", expiation.StatusCode);
            ViewData["TypeCode"] = new SelectList(_context.NoticeTypes, "TypeCode", "TypeCode", expiation.TypeCode);
            ViewData["WithdrawCode"] = new SelectList(_context.WithdrawnReasons, "WithdrawCode", "WithdrawCode", expiation.WithdrawCode);
            return View(expiation);
        }

        // POST: Expiations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DateOnly id, [Bind("ExpId,IncidentStartDate,IncidentStartTime,IssueDate,OffencePenaltyAmt,OffenceLevyAmt,CorporateFeeAmt,TotalFeeAmt,EnforceWarningNoticeFeeAmt,BacContentExp,VehicleSpeed,LocationSpeedLimit,OffenceCode,DriverState,RegState,LsaCode,CameraLocationId,CameraTypeCode,PhotoRejCode,StatusCode,WithdrawCode,TypeCode")] Expiation expiation)
        {
            if (id != expiation.IncidentStartDate)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expiation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpiationExists(expiation.IncidentStartDate))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CameraLocationId"] = new SelectList(_context.CameraCodes, "LocationId", "CameraTypeCode", expiation.CameraLocationId);
            ViewData["DriverState"] = new SelectList(_context.CountryStates, "CountryStateCode", "CountryStateCode", expiation.DriverState);
            ViewData["LsaCode"] = new SelectList(_context.LocalServiceAreas, "LsaCode", "LsaCode", expiation.LsaCode);
            ViewData["OffenceCode"] = new SelectList(_context.Offences, "OffenceCode", "OffenceCode", expiation.OffenceCode);
            ViewData["PhotoRejCode"] = new SelectList(_context.PhotoRejections, "RejectionCode", "RejectionCode", expiation.PhotoRejCode);
            ViewData["RegState"] = new SelectList(_context.CountryStates, "CountryStateCode", "CountryStateCode", expiation.RegState);
            ViewData["StatusCode"] = new SelectList(_context.NoticeStates, "StatusCode", "StatusCode", expiation.StatusCode);
            ViewData["TypeCode"] = new SelectList(_context.NoticeTypes, "TypeCode", "TypeCode", expiation.TypeCode);
            ViewData["WithdrawCode"] = new SelectList(_context.WithdrawnReasons, "WithdrawCode", "WithdrawCode", expiation.WithdrawCode);
            return View(expiation);
        }

        // GET: Expiations/Delete/5
        public async Task<IActionResult> Delete(DateOnly? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expiation = await _context.Expiations
                .Include(e => e.CameraCode)
                .Include(e => e.DriverStateNavigation)
                .Include(e => e.LsaCodeNavigation)
                .Include(e => e.OffenceCodeNavigation)
                .Include(e => e.PhotoRejCodeNavigation)
                .Include(e => e.RegStateNavigation)
                .Include(e => e.StatusCodeNavigation)
                .Include(e => e.TypeCodeNavigation)
                .Include(e => e.WithdrawCodeNavigation)
                .FirstOrDefaultAsync(m => m.IncidentStartDate == id);
            if (expiation == null)
            {
                return NotFound();
            }

            return View(expiation);
        }

        // POST: Expiations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DateOnly id)
        {
            var expiation = await _context.Expiations.FindAsync(id);
            if (expiation != null)
            {
                _context.Expiations.Remove(expiation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpiationExists(DateOnly id)
        {
            return _context.Expiations.Any(e => e.IncidentStartDate == id);
        }
    }
}
