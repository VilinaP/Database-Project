using Flower_Cafe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Flower_Cafe.Pages.MenuItemPages;

public class CreateModel : PageModel
{
    private readonly FlowerContext _context;

    public CreateModel(FlowerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = new();

    public List<SelectListItem> MenuItemTypeList { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadMenuItemTypesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // The form posts MenuItem.Type, not MenuItem.TypeNavigation.
        // TypeNavigation is an EF navigation property, so remove it from form validation.
        ModelState.Remove("MenuItem.TypeNavigation");
        ModelState.Remove("MenuItem.OrderItems");

        if (!ModelState.IsValid)
        {
            await LoadMenuItemTypesAsync();
            return Page();
        }

        var newMenuItem = new MenuItem
        {
            Name = MenuItem.Name,
            Type = MenuItem.Type,
            Price = MenuItem.Price
        };

        _context.MenuItems.Add(newMenuItem);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadMenuItemTypesAsync()
    {
        MenuItemTypeList = await _context.MenuItemTypes
            .Select(t => new SelectListItem
            {
                Value = t.MenuItemTypeid.ToString(),
                Text = t.TypeName
            })
            .ToListAsync();
    }
}