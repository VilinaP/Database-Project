using System;
using System.Collections.Generic;

namespace Flower_Cafe.Models;

public partial class Customer
{
    public long CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public long? Zip { get; set; }

    public long RewardPoints { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
