namespace Auth.Entities;

public static class ContentSecurityPolicy
{
    public const string DefaultSrc = "default-src 'self';";
    public const string ScriptSrc = "script-src 'self' 'unsafe-inline';";
    public const string StyleSrc = "style-src 'self' 'unsafe-inline';";
    public const string ImgSrc = "img-src 'self';";
    public const string FontSrc = "font-src 'self' https://fonts.scalar.com/;";
    public const string ConnectSrc = "connect-src 'self';";
    public const string ObjectSrc = "object-src 'none';";
}