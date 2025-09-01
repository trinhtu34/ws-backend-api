namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_PaymentStatusOrderTable
    {
        public long? OrderTableId { get; set; }
        public bool? IsSuccess { get; set; }
    }
    public class DTO_PaymentStatusCart
    {
        public long? CartId { get; set; }   
        public bool? IsSuccess { get; set; }
    }

}
