using System;
using System.Collections.Generic;

namespace Flower_Cafe.Models;

public partial class MenuItemType
{
    public long MenuItemTypeid { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
