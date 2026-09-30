using Flower_Cafe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Flower_Cafe.Pages.OrderPages;

public class DeleteModel : PageModel
{
    private readonly FlowerContext _context;

    public DeleteModel(FlowerContext context)
    {
        _context = context;
    }

    public Order Order { get; set; } = default!;

    public string CustomerName { get; set; } = "";

    public List<OrderItemDisplay>
    OrderItemsDisplay
    { get; set; } = new();

    public class OrderItemDisplay
    {
        public string Category { get; set; } = "";

        public string MenuItemName { get; set; } = "";

        public long Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal LineTotal { get; set; }
    }

    public async Task<IActionResult>
    OnGetAsync(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.MenuItem)
        .ThenInclude(mi => mi.TypeNavigation)
        .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        if (IsCustomerUser(out var customerId) && order.CustomerId != customerId)
        {
            return RedirectToPage("/AccessDenied");
        }

        Order = order;
        CustomerName = order.Customer.FirstName + " " + order.Customer.LastName;

        OrderItemsDisplay = order.OrderItems
        .Select(oi => new OrderItemDisplay
        {
            Category = oi.MenuItem.TypeNavigation.TypeName,
            MenuItemName = oi.MenuItem.Name,
            Quantity = oi.Quantity ?? 0,
            Price = oi.Price ?? 0,
            LineTotal = (oi.Price ?? 0) * (oi.Quantity ?? 0)
        })
        .OrderBy(oi => GetCategoryOrder(oi.Category))
        .ThenBy(oi => oi.MenuItemName)
        .ToList();

        return Page();
    }

    public async Task<IActionResult>
    OnPostAsync(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
        .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        if (IsCustomerUser(out var customerId) && order.CustomerId != customerId)
        {
            return RedirectToPage("/AccessDenied");
        }

        var orderItems = await _context.OrderItems
        .Where(oi => oi.OrderId == id)
        .ToListAsync();

        _context.OrderItems.RemoveRange(orderItems);
        _context.Orders.Remove(order);

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private bool IsCustomerUser(out long customerId)
    {
        customerId = 0;

        var userType = HttpContext.Session.GetString("UserType");
        var relatedIdText = HttpContext.Session.GetString("RelatedId");

        return userType == "customer" && long.TryParse(relatedIdText, out customerId);
    }

    private int GetCategoryOrder(string category)
    {
        return category switch
        {
            "Appetizer" => 1,
            "Entree" => 2,
            "Side" => 3,
            "Dessert" => 4,
            "Drink" => 5,
            "Flower" => 6,
            "Bouquet" => 7,
            "Potted Plant" => 8,
            _ => 99
        };
    }
}