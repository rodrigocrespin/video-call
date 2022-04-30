using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoCall.Application;

namespace VideoCall.Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IVideoCallService service;

        public AuthController(IVideoCallService service)
        {
            this.service = service;
        }

        [HttpGet("token")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> Get()
        {
            return Ok(new TokenResponse(service.GetJwt()));
        }

        [HttpGet("token/{identity}")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> Get(string identity)
        {
            return Ok(new TokenResponse(service.GetJwt(identity)));
        }
    }

    public record TokenResponse(string Token);
}
