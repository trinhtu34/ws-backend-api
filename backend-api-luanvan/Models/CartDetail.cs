using System;
using System.Collections.Generic;

namespace backend_api_luanvan.Models;

public partial class CartDetail
{
    public int CartDetailsId { get; set; }

    public long? CartId { get; set; }

    public string DishId { get; set; } = null!;

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Menu Dish { get; set; } = null!;
}
