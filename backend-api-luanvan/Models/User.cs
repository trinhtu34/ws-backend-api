﻿using System;
using System.Collections.Generic;

namespace backend_api_luanvan.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? UPassword { get; set; }

    public string? CustomerName { get; set; }

    public int RolesId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ContactForm> ContactForms { get; set; } = new List<ContactForm>();

    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    public virtual Role Roles { get; set; } = null!;
}
