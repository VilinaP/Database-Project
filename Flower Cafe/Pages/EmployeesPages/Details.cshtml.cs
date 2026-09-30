using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;

namespace Flower_Cafe.Pages.EmployeesPages;

public class DetailsModel : PageModel
{
    private readonly FlowerContext _context;

    public DetailsModel(FlowerContext context)
    {
        _context = context;
    }

    public Employees Employees { get; set; } = default!;

    public List<WageRate> WageRates { get; set; } = new();

    public WageRate? CurrentWageRate { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .Include(e => e.WageRates)
            .FirstOrDefaultAsync(e => e.EmployeesId == id);

        if (employee is null)
        {
            return NotFound();
        }

        Employees = employee;

        WageRates = employee.WageRates
            .OrderByDescending(w => w.StartDate)
            .ToList();

        CurrentWageRate = WageRates.FirstOrDefault();

        return Page();
    }
}