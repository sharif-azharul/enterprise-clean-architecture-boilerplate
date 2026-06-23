using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        public UsersController()
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateUserCommand command)
        {
            var id = await Mediator.Send(command);

            return Ok(
             ApiResponse<Guid>.SuccessResponse(
                 id,
                 "User created successfully"));
            }
    }
}
