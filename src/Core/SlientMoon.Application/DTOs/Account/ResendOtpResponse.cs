using System;

namespace SlientMoon.Application.DTOs.Account
{
    public class ResendOtpResponse
    {
        public string Message { get; set; }
        public DateTime OtpExpiresAt { get; set; }
    }
}
