using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Identities.Requests
{
    public class LoginRequest
    {
        [Required]
        public required string Username { get; set; } 
        [Required]
        public required string Password { get; set; } 
    }
}
