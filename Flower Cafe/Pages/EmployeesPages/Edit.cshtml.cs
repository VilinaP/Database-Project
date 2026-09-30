using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;

namespace Flower_Cafe.Pages.EmployeesPages;

public class EditModel : PageModel
{
    private readonly FlowerContext _context;

    public EditModel(FlowerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Employees Employees { get; set; } = new();

    [BindProperty]
    public decimal CurrentWage { get; set; }

    [BindProperty]
    public string CurrentWageType { get; set; } = "";

    [BindProperty]
    public DateOnly CurrentWageStartDate { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .Include(e => e.WageRates)
            .FirstOrDefaultAsync(e => e.EmployeesId == id);

        if (employee is null)
        {
            return NotFound();
        }

        Employees = employee;

        var currentWageRate = employee.WageRates
            .OrderByDescending(w => w.StartDate)
            .FirstOrDefault();

        if (currentWageRate != null)
        {
            CurrentWage = currentWageRate.Wage;
            CurrentWageType = currentWageRate.WageType;
            CurrentWageStartDate = currentWageRate.StartDate;
        }
        else
        {
            CurrentWage = 0.00m;
            CurrentWageType = "Hourly";
            CurrentWageStartDate = DateOnly.FromDateTime(DateTime.Today);
        }

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

        var employeeToUpdate = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeesId == Employees.EmployeesId);

        if (employeeToUpdate is null)
        {
            return NotFound();
        }

        employeeToUpdate.FirstName = Employees.FirstName;
        employeeToUpdate.LastName = Employees.LastName;
        employeeToUpdate.DateOfBirth = Employees.DateOfBirth;
        employeeToUpdate.Ssn = Employees.Ssn;
        employeeToUpdate.Street = Employees.Street;
        employeeToUpdate.City = Employees.City;
        employeeToUpdate.State = Employees.State;
        employeeToUpdate.Zip = Employees.Zip;

        var currentWageRate = await _context.WageRates
            .Where(w => w.EmployeeId == Employees.EmployeesId)
            .OrderByDescending(w => w.StartDate)
            .FirstOrDefaultAsync();

        if (currentWageRate == null)
        {
            var newWageRate = new WageRate
            {
                EmployeeId = Employees.EmployeesId,
                StartDate = CurrentWageStartDate,
                Wage = CurrentWage,
                WageType = CurrentWageType
            };

            _context.WageRates.Add(newWageRate);
        }
        else
        {
            currentWageRate.Wage = CurrentWage;
            currentWageRate.WageType = CurrentWageType;
            currentWageRate.StartDate = CurrentWageStartDate;
        }

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