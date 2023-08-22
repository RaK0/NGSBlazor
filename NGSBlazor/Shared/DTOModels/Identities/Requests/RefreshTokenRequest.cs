using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.DTOModels.Identities.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public required string Token { get; set; }
        [Required]
        public required string RefreshToken { get; set; }
    }
}
