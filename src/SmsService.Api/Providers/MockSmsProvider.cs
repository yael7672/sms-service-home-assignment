namespace SmsService.Api.Providers;

public sealed class MockSmsProvider : ISmsProvider
{
    public Task<ProviderSendResult> SendAsync(
        string to,
        string body,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(ProviderSendResult.Success($"mock-{Guid.NewGuid():N}"));
    }
}
