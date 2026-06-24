using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Authentication.DTOs
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
            = string.Empty;

        public string RefreshToken { get; set; }
            = string.Empty;
    }
}
