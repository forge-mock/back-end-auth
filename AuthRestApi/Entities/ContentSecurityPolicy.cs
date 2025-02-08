namespace Auth.Entities;

public static class ContentSecurityPolicy
{
    public const string DefaultSrc = "default-src 'self' 'unsafe-inline';";
    public const string ScriptSrc = "script-src 'self' 'unsafe-inline' 'unsafe-eval';";
    public const string StyleSrc = "style-src 'self' 'unsafe-inline';";
    public const string ImgSrc = "img-src 'self' 'unsafe-inline';";
    public const string FontSrc = "font-src 'self' https://fonts.scalar.com/;";
    public const string ConnectSrc = "connect-src 'self' 'unsafe-inline';";
    public const string ObjectSrc = "object-src 'none';";
}