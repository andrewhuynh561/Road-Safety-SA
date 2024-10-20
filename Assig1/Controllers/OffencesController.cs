﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assig1.Data;
using Assig1.Models;
using Assig1.ViewModels;
using System.Globalization;

namespace Assig1.Controllers
{
    public class OffencesController : Controller
    {
        private readonly ExpiationsContext _context;

        public OffencesController(ExpiationsContext context)
        {
            _context = context;
        }

        // GET: Offences
        public async Task<IActionResult> Index(OffenceSearchViewModel om)
        {
            if (om == null)
            {
                om =new OffenceSearchViewModel();
            }

            //Fetch categories
            var categories = _context.ExpiationCategories
                .Where(ec => ec.ParentCategoryId.HasValue)
                .OrderBy(ec => ec.CategoryName)
                .Select(ec => new {ec.CategoryId,ec.CategoryName})
                .ToList();

            om.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", om.CategoryId);

            var expiationsContext = _context.Offences
               .OrderBy(o =>o.Description)
               .AsQueryable();


            if (!string.IsNullOrWhiteSpace(om.SearchText))
            {
                expiationsContext = expiationsContext
                    .Where(o => o.Description.Contains(om.SearchText));

            }
            if (om.CategoryId != null)
            {
                expiationsContext = expiationsContext
                    .Where(i => i.Section.CategoryId == om.CategoryId.Value);
            }
            om.Offences = await expiationsContext.OrderBy(o => o.Description).ToListAsync();

            om.TotalOffences = om.Offences.Count;
            om.TotalFees = om.Offences.Sum(o => o.TotalFee ?? 0);

            return View(om);
            //#region Expiation Categories
            //ViewBag.Categories = new SelectList(await _context.ExpiationCategories.ToListAsync(), "CategoryId", "CategoryName");

            //#endregion


            //#region Offences query
            //ViewBag.SearchText = searchText;
            //var expiationsContext = _context.Offences
            //    .OrderBy(o =>o.Description)
            //    .AsQueryable();

            //if (!string.IsNullOrWhiteSpace(searchText))
            //{
            //    expiationsContext = expiationsContext
            //        .Where(o => o.Description.Contains(searchText));

            //}
            //if (CategoryId != null)
            //{
            //    expiationsContext = expiationsContext
            //        .Where(i => i.Section.CategoryId == CategoryId.Value);
            //}
            //#endregion
            //return View(await expiationsContext.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> GetOffenceData(int? categoryId)
        {
            // Fetch offences
            var offencesQuery = _context.Offences.AsQueryable();

            if (categoryId.HasValue)
            {
                offencesQuery = offencesQuery.Where(o => o.Section.CategoryId == categoryId.Value);
            }
            
            var data = await offencesQuery
                .Select(o => new
                {
                    o.Description,
                    ExpiationFee = o.ExpiationFee ?? 0,
                    TotalFee = o.TotalFee ?? 0
                })
                .ToListAsync();

            return Json(data);
        }

        // GET: Offences/Details/5
        public async Task<IActionResult> Details(string id, string statusFilter, int? year)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offence = await _context.Offences
                .Where(o => o.OffenceCode == id)
                .FirstOrDefaultAsync();

            if (offence == null)
            {
                return NotFound();
            }

            var section = await _context.Sections
                .Where(s => s.SectionId == offence.SectionId)
                .FirstOrDefaultAsync();

            var expiationsQuery = _context.Expiations
                .Where(e => e.OffenceCode == id);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                expiationsQuery = expiationsQuery.Where(e => e.StatusCode == statusFilter);
            }

            if (year.HasValue)
            {
                expiationsQuery = expiationsQuery.Where(e => e.IssueDate.HasValue && e.IssueDate.Value.Year == year.Value);
            }

            var expiations = await expiationsQuery
                .Select(e => new ExpiationDetailViewModel
                {
                    IncidentStartDate = e.IncidentStartDate,
                    IssueDate = e.IssueDate,
                    TotalFee = e.TotalFeeAmt,
                    Status = e.StatusCode
                })
                .ToListAsync();

            // Calculate average 
            var averageFee = expiations.Count > 0 ? expiations.Average(e => e.TotalFee ?? 0) : 0;

            var expiationsPerMonth = expiations
                .Where(e => e.IssueDate.HasValue)
                .GroupBy(e => e.IssueDate.Value.ToString("MMMM yyyy"))  // Group by month and year
                .ToDictionary(g => g.Key, g => g.Count());

            // Build the view model
            var viewModel = new OffenceDetailViewModel
            {
                OffenceCode = offence.OffenceCode,
                Description = offence.Description,
                SectionCode = section?.SectionCode, 
                TotalExpiations = expiations.Count,
                Expiations = expiations,
                StatusFilter = statusFilter,
                Year = year,
                AverageFee = averageFee,
                ExpiationsPerMonth = expiationsPerMonth
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetExpiationsPerMonthData(string offenceCode)
        {
            if (string.IsNullOrEmpty(offenceCode))
            {
                return NotFound();
            }

            var expiations = await _context.Expiations
                .Where(e => e.OffenceCode == offenceCode && e.IssueDate.HasValue)
                .ToListAsync();

            var expiationsPerMonth = expiations
                .GroupBy(e => e.IssueDate.Value.ToString("MMMM yyyy"))  // Group by month and year
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Month)  // Order by most recent month
                .ToList();

            return Json(expiationsPerMonth);
        }


        // GET: Offences/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "SectionId");
            return View();
        }

        // POST: Offences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OffenceCode,Description,ExpiationFee,AdultLevy,CorporateFee,TotalFee,DemeritPoints,SectionId,SectionCode")] Offence offence)
        {
            if (ModelState.IsValid)
            {
                _context.Add(offence);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "SectionId", offence.SectionId);
            return View(offence);
        }

        // GET: Offences/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offence = await _context.Offences.FindAsync(id);
            if (offence == null)
            {
                return NotFound();
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "SectionId", offence.SectionId);
            return View(offence);
        }

        // POST: Offences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OffenceCode,Description,ExpiationFee,AdultLevy,CorporateFee,TotalFee,DemeritPoints,SectionId,SectionCode")] Offence offence)
        {
            if (id != offence.OffenceCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offence);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OffenceExists(offence.OffenceCode))
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
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "SectionId", offence.SectionId);
            return View(offence);
        }

        // GET: Offences/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offence = await _context.Offences
                .Include(o => o.Section)
                .FirstOrDefaultAsync(m => m.OffenceCode == id);
            if (offence == null)
            {
                return NotFound();
            }

            return View(offence);
        }

        // POST: Offences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var offence = await _context.Offences.FindAsync(id);
            if (offence != null)
            {
                _context.Offences.Remove(offence);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OffenceExists(string id)
        {
            return _context.Offences.Any(e => e.OffenceCode == id);
        }
    }
}
