using System;
using System.Collections.Generic;

namespace Flower_Cafe.Models;

public partial class WageRate
{
    public long EmployeeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string WageType { get; set; } = null!;

    public decimal Wage { get; set; }

    public virtual Employees Employee { get; set; } = null!;
}
