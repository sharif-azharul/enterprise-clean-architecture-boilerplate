using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Common.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public IEnumerable<string>? Errors { get; set; }

        public static ApiResponse SuccessResponse(
            string message = "Success")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        public static ApiResponse FailureResponse(
            string message,
            IEnumerable<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
