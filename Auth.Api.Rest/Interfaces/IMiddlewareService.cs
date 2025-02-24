namespace Auth.Api.Rest.Interfaces;

public interface IMiddlewareService
{
    public void ConfigureHeaders(ref HttpContext context);
}