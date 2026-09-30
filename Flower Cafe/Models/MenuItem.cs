using System;
using System.Collections.Generic;

namespace Flower_Cafe.Models;

public partial class MenuItem
{
    public long MenuItemId { get; set; }

    public string Name { get; set; } = null!;

    public long Type { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual MenuItemType TypeNavigation { get; set; } = null!;
}
