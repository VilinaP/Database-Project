using Flower_Cafe.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Flower_Cafe.Pages.OrderPages;

public class CreateModel : PageModel
{
    private readonly FlowerContext _context;

    private const decimal SalesTaxRate = 0.0925m;

    public CreateModel(FlowerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public long CustomerId { get; set; }

    [BindProperty]
    public List<OrderLineInput> Items { get; set; } = new();

    public bool IsCustomerLogin { get; set; }

    public List<SelectListItem> CustomerList { get; set; } = new();

    public List<SelectListItem> CategoryList { get; set; } = new();

    public List<MenuItemOption> MenuItemOptions { get; set; } = new();

    public class OrderLineInput
    {
        public long? MenuItemId { get; set; }

        public long Quantity { get; set; } = 1;
    }

    public class MenuItemOption
    {
        public long MenuItemId { get; set; }

        public string Name { get; set; } = "";

        public decimal Price { get; set; }

        public long TypeId { get; set; }

        public string TypeName { get; set; } = "";
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Items.Add(new OrderLineInput { Quantity = 1 });

        if (IsCustomerUser(out var loggedInCustomerId))
        {
            CustomerId = loggedInCustomerId;
        }

        IsCustomerLogin = IsCustomerUser(out _);

        await LoadDropDownsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (IsCustomerUser(out var loggedInCustomerId))
        {
            CustomerId = loggedInCustomerId;
        }

        IsCustomerLogin = IsCustomerUser(out _);

        var selectedItems = Items
            .Where(i => i.MenuItemId.HasValue && i.Quantity > 0)
            .ToList();

        if (!selectedItems.Any())
        {
            ModelState.AddModelError(string.Empty, "Please select at least one menu item.");
        }

        if (!ModelState.IsValid)
        {
            if (!Items.Any())
            {
                Items.Add(new OrderLineInput { Quantity = 1 });
            }

            await LoadDropDownsAsync();
            return Page();
        }

        var customerExists = await _context.Customers
            .AnyAsync(c => c.CustomerId == CustomerId);

        if (!customerExists)
        {
            return NotFound();
        }

        var groupedItems = selectedItems
            .GroupBy(i => i.MenuItemId!.Value)
            .Select(g => new
            {
                MenuItemId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        var menuItemIds = groupedItems
            .Select(i => i.MenuItemId)
            .ToList();

        var menuItems = await _context.MenuItems
            .Where(m => menuItemIds.Contains(m.MenuItemId))
            .ToListAsync();

        decimal subtotal = 0.00m;

        foreach (var item in groupedItems)
        {
            var menuItem = menuItems.FirstOrDefault(m => m.MenuItemId == item.MenuItemId);

            if (menuItem == null)
            {
                return NotFound();
            }

            subtotal += menuItem.Price * item.Quantity;
        }

        decimal salesTax = subtotal * SalesTaxRate;
        decimal total = subtotal + salesTax;

        var order = new Order
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            CustomerId = CustomerId,
            SubTotal = subtotal,
            SalesTax = salesTax,
            Total = total
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in groupedItems)
        {
            var menuItem = menuItems.First(m => m.MenuItemId == item.MenuItemId);

            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                Price = menuItem.Price
            };

            _context.OrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    public long? GetTypeIdForMenuItem(long? menuItemId)
    {
        if (menuItemId == null)
        {
            return null;
        }

        return MenuItemOptions
            .FirstOrDefault(m => m.MenuItemId == menuItemId.Value)
            ?.TypeId;
    }

    private bool IsCustomerUser(out long customerId)
    {
        customerId = 0;

        var userType = HttpContext.Session.GetString("UserType");
        var relatedIdText = HttpContext.Session.GetString("RelatedId");

        return userType == "customer" && long.TryParse(relatedIdText, out customerId);
    }

    private async Task LoadDropDownsAsync()
    {
        var categoryOrder = new Dictionary<string, int>
        {
            { "Appetizer", 1 },
            { "Entree", 2 },
            { "Side", 3 },
            { "Dessert", 4 },
            { "Drink", 5 },
            { "Flower", 6 },
            { "Bouquet", 7 },
            { "Potted Plant", 8 }
        };

        CustomerList = await _context.Customers
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new SelectListItem
            {
                Value = c.CustomerId.ToString(),
                Text = c.FirstName + " " + c.LastName,
                Selected = c.CustomerId == CustomerId
            })
            .ToListAsync();

        var menuItemTypes = await _context.MenuItemTypes.ToListAsync();

        CategoryList = menuItemTypes
            .OrderBy(t => categoryOrder.ContainsKey(t.TypeName) ? categoryOrder[t.TypeName] : 99)
            .ThenBy(t => t.TypeName)
            .Select(t => new SelectListItem
            {
                Value = t.MenuItemTypeid.ToString(),
                Text = t.TypeName
            })
            .ToList();

        var menuItems = await _context.MenuItems
            .Include(m => m.TypeNavigation)
            .ToListAsync();

        MenuItemOptions = menuItems
            .OrderBy(m => categoryOrder.ContainsKey(m.TypeNavigation.TypeName) ? categoryOrder[m.TypeNavigation.TypeName] : 99)
            .ThenBy(m => m.Name)
            .Select(m => new MenuItemOption
            {
                MenuItemId = m.MenuItemId,
                Name = m.Name,
                Price = m.Price,
                TypeId = m.Type,
                TypeName = m.TypeNavigation.TypeName
            })
            .ToList();
    }
}