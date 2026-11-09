namespace SmsService.Api.Providers;

public sealed class SmsProviderOptions
{
    public const string SectionName = "SmsProvider";
    public bool UseMockProvider { get; init; }
    public string BaseUrl { get; init; } = string.Empty;
}
