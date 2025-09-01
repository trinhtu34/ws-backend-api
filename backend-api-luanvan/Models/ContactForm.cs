using System;
using System.Collections.Generic;

namespace backend_api_luanvan.Models;

public partial class ContactForm
{
    public int ContactId { get; set; }

    public string UserId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public virtual User User { get; set; } = null!;
}
