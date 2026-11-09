namespace SmsService.Api.Providers;

public interface ISmsProvider
{
    Task<ProviderSendResult> SendAsync(
        string to,
        string body,
        CancellationToken cancellationToken = default);
}

public sealed record ProviderSendResult(
    bool IsSuccess,
    string? ProviderMessageId,
    string? ErrorMessage)
{
    public static ProviderSendResult Success(string providerMessageId) =>
        new(true, providerMessageId, null);

    public static ProviderSendResult Failure(string errorMessage) =>
        new(false, null, errorMessage);
}
