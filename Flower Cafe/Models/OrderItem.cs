using System;
using System.Collections.Generic;

namespace Flower_Cafe.Models;

public partial class OrderItem
{
    public long OrderId { get; set; }

    public long MenuItemId { get; set; }

    public long? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
