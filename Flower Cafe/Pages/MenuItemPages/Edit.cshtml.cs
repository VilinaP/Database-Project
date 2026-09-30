using Flower_Cafe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Flower_Cafe.Pages.MenuItemPages;

public class EditModel : PageModel
{
    private readonly FlowerContext _context;

    public EditModel(FlowerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = new();

    public List<SelectListItem> MenuItemTypeList { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(long? id)
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

        MenuItem = menuItem;

        await LoadMenuItemTypesAsync(MenuItem.Type);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // These are navigation properties. The form does not submit these.
        ModelState.Remove("MenuItem.TypeNavigation");
        ModelState.Remove("MenuItem.OrderItems");

        if (!ModelState.IsValid)
        {
            foreach (var modelStateEntry in ModelState)
            {
                foreach (var error in modelStateEntry.Value.Errors)
                {
                    Console.WriteLine($"{modelStateEntry.Key}: {error.ErrorMessage}");
                }
            }

            await LoadMenuItemTypesAsync(MenuItem.Type);
            return Page();
        }

        var menuItemToUpdate = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.MenuItemId == MenuItem.MenuItemId);

        if (menuItemToUpdate == null)
        {
            return NotFound();
        }

        menuItemToUpdate.Name = MenuItem.Name;
        menuItemToUpdate.Type = MenuItem.Type;
        menuItemToUpdate.Price = MenuItem.Price;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadMenuItemTypesAsync(long selectedTypeId)
    {
        var menuItemTypes = await _context.MenuItemTypes.ToListAsync();

        MenuItemTypeList = menuItemTypes
            .Select(t => new SelectListItem
            {
                Value = t.MenuItemTypeid.ToString(),
                Text = t.TypeName,
                Selected = t.MenuItemTypeid == selectedTypeId
            })
            .ToList();
    }
}