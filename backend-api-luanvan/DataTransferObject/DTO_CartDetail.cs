namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_CartDetail
    {
        public int CartDetailsId { get; set; }

        public long? CartId { get; set; }

        public string DishId { get; set; } = null!;

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }
    public class DTO_CartDetail_WithName
    {
        public int CartDetailsId { get; set; }

        public long? CartId { get; set; }

        public string DishId { get; set; } = null!;
        public string DishName { get; set; } = null!;

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }
}
