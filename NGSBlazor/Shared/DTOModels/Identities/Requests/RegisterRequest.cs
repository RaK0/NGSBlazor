using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.DTOModels.Identities.Requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(2)]
        public string UserName { get; set; } = "";
        [Required]
        [MinLength(5)]
        public string Password { get; set; } = "";
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = "";
    }
}
