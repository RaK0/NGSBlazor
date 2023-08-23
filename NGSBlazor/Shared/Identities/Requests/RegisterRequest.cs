using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Identities.Requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(2)]
        public required string UserName { get; set; } 
        [Required]
        [MinLength(5)]
        public required string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public required string ConfirmPassword { get; set; }
    }
}
