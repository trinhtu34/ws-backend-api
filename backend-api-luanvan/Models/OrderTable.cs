using System;
using System.Collections.Generic;

namespace backend_api_luanvan.Models;

public partial class OrderTable
{
    public long OrderTableId { get; set; }

    public string? UserId { get; set; }

    public DateTime StartingTime { get; set; }

    public bool? IsCancel { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? TotalDeposit { get; set; }

    public DateTime OrderDate { get; set; }

    public virtual ICollection<OrderFoodDetail> OrderFoodDetails { get; set; } = new List<OrderFoodDetail>();

    public virtual ICollection<OrderTablesDetail> OrderTablesDetails { get; set; } = new List<OrderTablesDetail>();

    public virtual ICollection<PaymentResult> PaymentResults { get; set; } = new List<PaymentResult>();

    public virtual User? User { get; set; }
}
