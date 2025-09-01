namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_Payment
    {
        public long PaymentResultId { get; set; }

        public long? OrderTableId { get; set; }

        public long? CartId { get; set; }

        public decimal? Amount { get; set; }

        public long? PaymentId { get; set; }

        public bool? IsSuccess { get; set; }

        public string? Description { get; set; }

        public DateTime? Timestamp { get; set; }

        public long? VnpayTransactionId { get; set; }

        public string? PaymentMethod { get; set; }

        public string? BankCode { get; set; }

        public string? BankTransactionId { get; set; }

        public string? ResponseDescription { get; set; }

        public string? TransactionStatusDescription { get; set; }
    }
    public class DTO_PaymentResultInfoTotal
    {
        public long PaymentResultId { get; set; }

        public long? OrderTableId { get; set; }
        public long? CartId { get; set; }
        public decimal? Amount { get; set; }
        public bool? IsSuccess { get; set; }

        public string? Description { get; set; }

        public DateTime? Timestamp { get; set; }

        public string? PaymentMethod { get; set; }
        public string? BankCode { get; set; }
        public string? ResponseDescription { get; set; }

        public string? TransactionStatusDescription { get; set; }
    }
    public class DTO_Payment_ShowHistoryPayment_Cart
    {
        public long PaymentResultId { get; set; }
        public long? CartId { get; set; }

        public decimal? Amount { get; set; }

        public bool? IsSuccess { get; set; }

        public DateTime? Timestamp { get; set; }
        public string? PaymentMethod { get; set; }

        public string? BankCode { get; set; }

        public string? ResponseDescription { get; set; }

        public string? TransactionStatusDescription { get; set; }
    }
    public class DTO_Payment_ShowHistoryPayment_OrderTable
    {
        public long PaymentResultId { get; set; }

        public long? OrderTableId { get; set; }

        public decimal? Amount { get; set; }

        public bool? IsSuccess { get; set; }

        public DateTime? Timestamp { get; set; }
        public string? PaymentMethod { get; set; }

        public string? BankCode { get; set; }

        public string? ResponseDescription { get; set; }

        public string? TransactionStatusDescription { get; set; }
    }
    public class DTO_Payment_OrderTable
    {
        public long PaymentResultId { get; set; }

        public long? OrderTableId { get; set; }

        public decimal? Amount { get; set; }

        public long? PaymentId { get; set; }

        public bool? IsSuccess { get; set; }

        public string? Description { get; set; }

        public DateTime? Timestamp { get; set; }

        public string? PaymentMethod { get; set; }
    }
    public class DTO_Payment_Cart
    {
        public long PaymentResultId { get; set; }

        public long? CartId { get; set; }

        public decimal? Amount { get; set; }

        public long? PaymentId { get; set; }

        public bool? IsSuccess { get; set; }

        public string? Description { get; set; }

        public DateTime? Timestamp { get; set; }

        public string? PaymentMethod { get; set; }
    }
}
