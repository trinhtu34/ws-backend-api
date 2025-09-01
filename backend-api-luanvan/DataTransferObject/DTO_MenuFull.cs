namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_MenuFull
    {
        public string DishId { get; set; } = null!;

        public string DishName { get; set; } = null!;

        public decimal Price { get; set; }

        public string? Descriptions { get; set; }

        public int CategoryId { get; set; }

        public int RegionId { get; set; }

        public string? Images { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
