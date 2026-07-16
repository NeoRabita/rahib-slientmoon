using SlientMoon.Domain.Common;
using SlientMoon.Domain.Enums;
using System;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class ApplicationUser : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailVerified { get; set; }
        public string? AvatarUrl { get; set; }
        public LoginType LoginType { get; set; } = LoginType.Normal;

        public string? RefreshTokenId { get; set; }
        public RefreshToken RefreshToken { get; set; }

        public ICollection<UserTopic> UserTopics { get; set; } = new List<UserTopic>();
    }

}
