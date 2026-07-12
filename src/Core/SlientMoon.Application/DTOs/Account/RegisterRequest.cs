using System.ComponentModel.DataAnnotations;

namespace SlientMoon.Application.DTOs.Account
{
    public class RegisterRequest
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}