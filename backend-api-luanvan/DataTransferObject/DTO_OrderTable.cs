namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_OrderTable
    {
        public long OrderTableId { get; set; }

        public string? UserId { get; set; }

        public DateTime StartingTime { get; set; }

        public bool? IsCancel { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? TotalDeposit { get; set; }

        public DateTime OrderDate { get; set; }
    }
    public class DTO_OrderTable_Paymentstatus
    {
        public long OrderTableId { get; set; }

        public string? UserId { get; set; }

        public DateTime StartingTime { get; set; }

        public bool? IsCancel { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? TotalDeposit { get; set; }

        public DateTime OrderDate { get; set; }
        public bool IsPaid { get; set; }
    }
    public class DTO_OrderTable_Paymentstatus_Food_payment_info
    {
        public long OrderTableId { get; set; }

        public string? UserId { get; set; }

        public DateTime StartingTime { get; set; }

        public bool? IsCancel { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? TotalDeposit { get; set; }

        public DateTime OrderDate { get; set; }
        public bool IsPaidDeposit { get; set; }
        public bool IsPaidTotalPrice { get; set; }
    }

    public class DTO_ordertable_detail_and_food
    {
        public long OrderTableId { get; set; }

        public string? UserId { get; set; }

        public DateTime StartingTime { get; set; }

        public bool? IsCancel { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? TotalDeposit { get; set; }

        public DateTime OrderDate { get; set; }
        public List<DTO_OrderTablesDetail> OrderTableDetails { get; set; } = new List<DTO_OrderTablesDetail>();
        public List<DTO_OrderFoodDetail> OrderFoodDetails { get; set; } = new List<DTO_OrderFoodDetail>();
    }
}
