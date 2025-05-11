using Auth.Application.DTOs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("csrf")]
public class CsrfController(IAntiforgery antiforgery) : ControllerBase
{
    [HttpGet("token")]
    public IActionResult Token()
    {
        AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(HttpContext);
        Response.Cookies.Append(
            "XSRF-TOKEN",
            tokens.RequestToken!,
            new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });

        return Ok(new ResultSuccessDto<string>(true, "CSRF token generated"));
    }
}