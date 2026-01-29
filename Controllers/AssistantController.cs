using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EShift.Data;
using EShift.Models;
using DocumentFormat.OpenXml.Wordprocessing;

public class AssistantController : Controller
{
    private readonly ApplicationDbContext _context;
    public AssistantController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        var assistants = from a in _context.Assistants
                         select a;

        if (!string.IsNullOrEmpty(searchString))
        {
            assistants = assistants.Where(a => a.AssistantName.Contains(searchString));
        }
        ViewData["CurrentFilter"] = searchString;

        return View("~/Views/AdminDashboard/Assistant.cshtml", await assistants.ToListAsync());
    }


    public IActionResult Create()
    {
        return View("~/Views/AdminDashboard/AssistantCreate.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Assistant assistant)
    {
        if (ModelState.IsValid)
        {
            _context.Assistants.Add(assistant);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View("~/Views/AdminDashboard/AssistantCreate.cshtml", assistant);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var assistant = await _context.Assistants.FindAsync(id);
        if (assistant == null) return NotFound();
        return View("~/Views/AdminDashboard/AssistantEdit.cshtml", assistant);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Assistant assistant)
    {
        if (id != assistant.AssistantID) return NotFound();

        var existing = await _context.Assistants.FindAsync(id);
        if (existing == null) return NotFound();

        if (existing.Status == "Busy")
        {
            ModelState.AddModelError("", "Cannot edit while assistant is busy.");
            return View("~/Views/AdminDashboard/AssistantEdit.cshtml", assistant);
        }

        existing.AssistantName = assistant.AssistantName;
        existing.Status = assistant.Status;
        existing.Role = assistant.Role;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var assistant = await _context.Assistants.FindAsync(id);
        if (assistant == null) return NotFound();
        return View("~/Views/AdminDashboard/AssistantDelete.cshtml", assistant);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var assistant = await _context.Assistants.FindAsync(id);
        if (assistant != null)
        {
            _context.Assistants.Remove(assistant);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
