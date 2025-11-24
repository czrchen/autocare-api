using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace autocare_api.Models
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UsedAt { get; set; }

        [NotMapped]
        public bool IsUsed => UsedAt.HasValue;
    }
}
        