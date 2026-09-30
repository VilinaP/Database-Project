using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;

namespace Flower_Cafe.Pages.MenuItemPages;

public class DetailsModel : PageModel
{
    private readonly FlowerContext _context;
    public DetailsModel(FlowerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = default!;
    public string typename { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var menuitem = await _context.MenuItems
            .Include(m => m.TypeNavigation)
            .FirstOrDefaultAsync(m => m.MenuItemId == id);

        if (menuitem is null)
        {
            return NotFound();
        }
        else
        {
            MenuItem = menuitem;
        }

        MenuItem = menuitem;
        typename = menuitem.TypeNavigation.TypeName;

        return Page();
    }
}
