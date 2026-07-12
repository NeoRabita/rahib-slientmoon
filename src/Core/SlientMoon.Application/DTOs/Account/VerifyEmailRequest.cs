using System.ComponentModel.DataAnnotations;

namespace SlientMoon.Application.DTOs.Account
{
    public class VerifyEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Otp { get; set; }
    }
}
