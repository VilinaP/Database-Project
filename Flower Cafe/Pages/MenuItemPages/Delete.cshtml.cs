using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;

namespace Flower_Cafe.Pages.MenuItemPages;

public class DeleteModel : PageModel
{
    private readonly FlowerContext _context;

    public DeleteModel(FlowerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = default!;

    public int RelatedOrderItemCount { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var menuItem = await _context.MenuItems
            .Include(m => m.TypeNavigation)
            .FirstOrDefaultAsync(m => m.MenuItemId == id);

        if (menuItem == null)
        {
            return NotFound();
        }

        MenuItem = menuItem;

        RelatedOrderItemCount = await _context.OrderItems
            .CountAsync(oi => oi.MenuItemId == id);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.MenuItemId == id);

        if (menuItem == null)
        {
            return NotFound();
        }

        var relatedOrderItems = await _context.OrderItems
            .Where(oi => oi.MenuItemId == id)
            .ToListAsync();

        if (relatedOrderItems.Any())
        {
            _context.OrderItems.RemoveRange(relatedOrderItems);
        }

        _context.MenuItems.Remove(menuItem);

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}