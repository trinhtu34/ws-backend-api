namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_ContactForm
    {
        public int ContactId { get; set; }

        public string UserId { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime? CreateAt { get; set; }
    }
}
