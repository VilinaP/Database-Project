using Flower_Cafe.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Flower_Cafe.Pages.OrderPages;

public class IndexModel : PageModel
{
    private readonly FlowerContext _context;

    public IndexModel(FlowerContext context)
    {
        _context = context;
    }

    public IList<Order> Order { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (IsCustomerUser(out var loggedInCustomerId))
        {
            Order = await _context.Orders
                .Where(o => o.CustomerId == loggedInCustomerId)
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.OrderId)
                .ToListAsync();
        }
        else
        {
            Order = await _context.Orders
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.OrderId)
                .ToListAsync();
        }
    }

    private bool IsCustomerUser(out long customerId)
    {
        customerId = 0;

        var userType = HttpContext.Session.GetString("UserType");
        var relatedIdText = HttpContext.Session.GetString("RelatedId");

        return userType == "customer" && long.TryParse(relatedIdText, out customerId);
    }
}