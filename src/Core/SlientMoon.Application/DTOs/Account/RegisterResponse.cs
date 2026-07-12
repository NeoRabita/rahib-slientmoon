using System;

namespace SlientMoon.Application.DTOs.Account
{
    public class RegisterResponse
    {
        public string Message { get; set; }
        public string Email { get; set; }
        public DateTime OtpExpiresAt { get; set; }
    }
}
