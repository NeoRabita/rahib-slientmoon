using System;
using System.ComponentModel.DataAnnotations.Schema;
using SlientMoon.Domain.Common;

namespace SlientMoon.Domain.Entities
{
    [Table("RefreshTokens")]
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;

        [NotMapped]
        public bool IsRevoked => Revoked != null;

        [NotMapped]
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}