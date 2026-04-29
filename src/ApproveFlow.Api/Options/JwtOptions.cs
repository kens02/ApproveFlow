namespace ApproveFlow.Api.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = "ApproveFlow";
    public string Audience { get; init; } = "ApproveFlow.Client";
    public string Secret { get; init; } = "ChangeMe_AtLeast32Chars_LongSecretKey";
}
