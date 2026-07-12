using System.ComponentModel.DataAnnotations;

namespace SlientMoon.Application.DTOs.Account
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}