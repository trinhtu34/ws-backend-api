namespace backend_api_luanvan.DataTransferObject
{
    public class DTO_User
    {
        public string UserId { get; set; } = null!;

        public string? UPassword { get; set; }

        public string? CustomerName { get; set; }

        public int RolesId { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateTime? CreateAt { get; set; }
    }
    public class DTO_User_Guest
    {
        public string UserId { get; set; } = null!;

        public string? CustomerName { get; set; }

        public int RolesId { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
