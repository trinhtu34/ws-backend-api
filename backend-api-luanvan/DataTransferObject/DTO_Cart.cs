namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_Cart
    {
        public long CartId { get; set; }

        public string UserId { get; set; } = null!;

        public DateTime? OrderTime { get; set; }
        public decimal? TotalPrice { get; set; }

        public bool? IsCancel { get; set; }
    }
    public class DTO_Cart_HaveFinishInfo
    {
        public long CartId { get; set; }

        public string UserId { get; set; } = null!;

        public DateTime? OrderTime { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? IsFinish { get; set; }
        public bool? IsCancel { get; set; }
    }
    public class DTO_Cart_WithPaymentInfo_andIsFinish
    {
        public long CartId { get; set; }

        public string UserId { get; set; } = null!;

        public DateTime? OrderTime { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? IsCancel { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool? IsFinish { get; set; }
    }
    public class DTO_Cart_Cancel_Status
    {
        public long CartId { get; set; }
        public bool? IsCancel { get; set; }
    }
    public class DTO_Cart_Finish_Status
    {
        public long CartId { get; set; }
        public bool? IsFinish { get; set; }
    }
}
