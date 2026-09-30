using System;
using System.Collections.Generic;

namespace Flower_Cafe.Models;

public partial class Order
{
    public long OrderId { get; set; }

    public DateOnly? Date { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? SalesTax { get; set; }

    public decimal? Total { get; set; }

    public long CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}