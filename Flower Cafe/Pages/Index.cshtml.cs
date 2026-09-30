using Flower_Cafe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Flower_Cafe.Pages;

public class IndexModel : PageModel
{
    private readonly FlowerContext _context;

    public IndexModel(FlowerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    public string ErrorMessage { get; set; } = "";

    public bool IsLoggedIn { get; set; }

    public string LoggedInUsername { get; set; } = "";

    public string LoggedInRole { get; set; } = "";

    public void OnGet()
    {
        IsLoggedIn = AuthHelper.IsLoggedIn(HttpContext);
        LoggedInUsername = AuthHelper.GetUsername(HttpContext);
        LoggedInRole = AuthHelper.GetUserType(HttpContext);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == Username);

        if (user == null)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        bool passwordMatches = AuthHelper.PasswordMatches(
            user.PasswordHash,
            user.PasswordSalt,
            Password
        );

        if (!passwordMatches)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("UserType", user.UserType);

        if (user.RelatedId != null)
        {
            HttpContext.Session.SetString("RelatedId", user.RelatedId.Value.ToString());
        }
        else
        {
            HttpContext.Session.Remove("RelatedId");
        }

        return RedirectToPage("/Index");
    }
}