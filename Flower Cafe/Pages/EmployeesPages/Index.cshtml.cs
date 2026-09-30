using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;

namespace Flower_Cafe.Pages.EmployeesPages;

public class IndexModel : PageModel
{
    private readonly FlowerContext _context;

    public IndexModel(FlowerContext context)
    {
        _context = context;
    }

    public IList<Employees> Employees { get; set; } = new List<Employees>();

    public async Task OnGetAsync()
    {
        Employees = await _context.Employees
            .Include(e => e.WageRates)
            .ToListAsync();
    }
}