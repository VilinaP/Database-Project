using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;

namespace Flower_Cafe.Pages.MenuItemPages;

public class IndexModel : PageModel
{
    private readonly FlowerContext _context;

    public IndexModel(FlowerContext context)
    {
        _context = context;
    }

    public IList<MenuItem> MenuItem { get; set; } = new List<MenuItem>();

    public async Task OnGetAsync()
    {
        var menuItems = await _context.MenuItems
            .Include(m => m.TypeNavigation)
            .ToListAsync();

        var categoryOrder = new Dictionary<string, int>
    {
        { "Appetizer", 1 },
        { "Entree", 2 },
        { "Dessert", 3 },
        { "Drink", 4 },
        { "Flower", 5 },
        { "Bouquet", 6 },
        { "Potted Plant", 7 }
    };

        MenuItem = menuItems
            .OrderBy(m => categoryOrder.ContainsKey(m.TypeNavigation.TypeName)
                ? categoryOrder[m.TypeNavigation.TypeName]
                : 99)
            .ThenBy(m => m.Name)
            .ToList();
    }
}