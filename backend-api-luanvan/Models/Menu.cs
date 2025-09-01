using System;
using System.Collections.Generic;

namespace backend_api_luanvan.Models;

public partial class Menu
{
    public string DishId { get; set; } = null!;

    public string DishName { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Descriptions { get; set; }

    public bool? IsAvailable { get; set; }

    public int CategoryId { get; set; }

    public int RegionId { get; set; }

    public string? Images { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderFoodDetail> OrderFoodDetails { get; set; } = new List<OrderFoodDetail>();

    public virtual Region Region { get; set; } = null!;
}
