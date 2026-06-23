using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.Api.Controllers
{
    
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        private ISender? _mediator;

        protected ISender Mediator =>
            _mediator ??=
            HttpContext.RequestServices
                .GetRequiredService<ISender>();
    }
}
