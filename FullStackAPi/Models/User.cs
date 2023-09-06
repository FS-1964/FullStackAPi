namespace FullStackAPi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Mobile { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? CreatedAt { get; set; } = string.Empty;
        public string? ModifiedAt { get; set; } = string.Empty;
        public string? Token { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
    }
}
