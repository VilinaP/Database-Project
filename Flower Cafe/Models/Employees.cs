using System;
using System.Collections.Generic;

namespace Flower_Cafe.Models;

public partial class Employees
{
    public long EmployeesId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Ssn { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public long Zip { get; set; }

    public virtual ICollection<WageRate> WageRates { get; set; } = new List<WageRate>();
}
