using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;

namespace Flower_Cafe.Pages.EmployeesPages;

public class CreateModel : PageModel
{
    private readonly FlowerContext _context;

    public CreateModel(FlowerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Employees Employees { get; set; } = new();

    [BindProperty]
    public decimal CurrentWage { get; set; }

    [BindProperty]
    public string CurrentWageType { get; set; } = "Hourly";

    [BindProperty]
    public DateOnly CurrentWageStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public IActionResult OnGet()
    {
        CurrentWageType = "Hourly";
        CurrentWageStartDate = DateOnly.FromDateTime(DateTime.Today);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("Employees.WageRates");

        if (CurrentWage < 0)
        {
            ModelState.AddModelError("CurrentWage", "Wage cannot be negative.");
        }

        if (CurrentWage > 999999.99m)
        {
            ModelState.AddModelError("CurrentWage", "Wage is too large. Enter a value less than 1,000,000.");
        }

        if (string.IsNullOrWhiteSpace(CurrentWageType))
        {
            ModelState.AddModelError("CurrentWageType", "Wage type is required.");
        }

        if (CurrentWageStartDate == default)
        {
            ModelState.AddModelError("CurrentWageStartDate", "Wage start date is required.");
        }

        if (Employees.Zip < 0)
        {
            ModelState.AddModelError("Employees.Zip", "Zip code cannot be negative.");
        }

        if (!string.IsNullOrWhiteSpace(Employees.State) && Employees.State.Length > 2)
        {
            ModelState.AddModelError("Employees.State", "State must be a 2-letter abbreviation.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Employees.Add(Employees);
        await _context.SaveChangesAsync();

        var wageRate = new WageRate
        {
            EmployeeId = Employees.EmployeesId,
            StartDate = CurrentWageStartDate,
            Wage = CurrentWage,
            WageType = CurrentWageType
        };

        _context.WageRates.Add(wageRate);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "One or more values are outside the allowed database range. Please check the wage, zip code, dates, and other fields.");
            return Page();
        }

        return RedirectToPage("./Index");
    }
}