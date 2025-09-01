namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_Table
    {
        public int TableId { get; set; }

        public int? Capacity { get; set; }

        public decimal? Deposit { get; set; }

        public string? Description { get; set; }

        public int RegionId { get; set; }
    }
}
