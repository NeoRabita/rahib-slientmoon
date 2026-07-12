using System;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class OtpData
    {
        public string Code { get; set; }
        public int Attempts { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
