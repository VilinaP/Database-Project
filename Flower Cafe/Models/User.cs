using System;
using System.Collections.Generic;

namespace Flower_Cafe.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public long? RelatedId { get; set; }
}
