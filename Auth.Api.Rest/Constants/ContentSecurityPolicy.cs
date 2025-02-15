namespace Auth.Constants;

internal static class ContentSecurityPolicy
{
    internal const string DefaultSrc = "default-src 'self';";
    internal const string ScriptSrc = "script-src 'self' 'unsafe-inline';";
    internal const string StyleSrc = "style-src 'self' 'unsafe-inline';";
    internal const string ImgSrc = "img-src 'self';";
    internal const string FontSrc = "font-src 'self' https://fonts.scalar.com/;";
    internal const string ConnectSrc = "connect-src 'self';";
    internal const string ObjectSrc = "object-src 'none';";
}