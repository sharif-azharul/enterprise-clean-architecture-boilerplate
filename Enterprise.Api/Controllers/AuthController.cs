using Enterprise.Application.Features.Authentication.Login;
using Enterprise.Application.Features.Authentication.Register;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
