using System;
using System.ComponentModel.DataAnnotations;

namespace SlientMoon.Application.DTOs.Email
{
    public class ConfirmEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
    }
}