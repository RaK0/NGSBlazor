using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.DTOModels.Identities.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } 
        [Required]
        public string Password { get; set; } 
    }
}
