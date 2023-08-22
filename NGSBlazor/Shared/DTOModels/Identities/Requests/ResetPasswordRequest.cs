using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.DTOModels.Identities.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public required string ConfirmPassword { get; set; }
        [Required]
        public required string Token { get; set; }
    }
}
