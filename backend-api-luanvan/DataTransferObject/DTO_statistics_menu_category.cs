namespace backend_api_luanvan.DataTransferObject
{
    public class DishRevenueDto
    {
        public string DishId { get; set; } = null!;
        public string DishName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class CategoryRevenueDto
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int DishCount { get; set; }
        public int OrderCount { get; set; }
    }

    public class DateRevenueDto
    {
        public DateTime Date { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
        public int DishCount { get; set; }
    }

    public class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalQuantitySold { get; set; }
        public int TotalOrders { get; set; }
        public int TotalDishes { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class DishRevenueDetailDto
    {
        public string DishId { get; set; } = null!;
        public string DishName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
        public DateTime FirstOrderDate { get; set; }
        public DateTime LastOrderDate { get; set; }
        public decimal AverageQuantityPerOrder { get; set; }
    }
}
