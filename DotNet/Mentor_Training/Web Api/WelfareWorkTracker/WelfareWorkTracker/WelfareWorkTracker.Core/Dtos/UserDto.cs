namespace WelfareWorkTracker.Core.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Age { get; set; }
        public long MobileNumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? RoleName { get; set; }
        public string? ConstituencyName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

    }
}
