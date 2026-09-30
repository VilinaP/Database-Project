using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;

namespace Flower_Cafe.Pages.EmployeesPages;

public class DeleteModel : PageModel
{
    private readonly FlowerContext _context;

    public DeleteModel(FlowerContext context)
    {
        _context = context;
    }

    public Employees Employees { get; set; } = default!;

    public List<WageRate> WageRates { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var employees = await _context.Employees
            .Include(e => e.WageRates)
            .FirstOrDefaultAsync(e => e.EmployeesId == id);

        if (employees is null)
        {
            return NotFound();
        }

        Employees = employees;

        WageRates = employees.WageRates
            .OrderByDescending(w => w.StartDate)
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeesId == id);

        if (employee is null)
        {
            return NotFound();
        }

        var wageRates = await _context.WageRates
            .Where(w => w.EmployeeId == id)
            .ToListAsync();

        _context.WageRates.RemoveRange(wageRates);
        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}