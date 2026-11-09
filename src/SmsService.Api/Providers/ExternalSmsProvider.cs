using System.Net.Http.Json;

namespace SmsService.Api.Providers;

public sealed class ExternalSmsProvider(HttpClient httpClient) : ISmsProvider
{
    public async Task<ProviderSendResult> SendAsync(
        string to,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                "messages",
                new ProviderSendRequest(to, body),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return ProviderSendResult.Failure(
                    $"Provider returned HTTP {(int)response.StatusCode}.");
            }

            var result = await response.Content.ReadFromJsonAsync<ProviderSendResponse>(
                cancellationToken: cancellationToken);

            return string.IsNullOrWhiteSpace(result?.Id)
                ? ProviderSendResult.Failure("Provider response did not contain a message id.")
                : ProviderSendResult.Success(result.Id);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return ProviderSendResult.Failure("Provider request timed out.");
        }
        catch (HttpRequestException exception)
        {
            return ProviderSendResult.Failure($"Provider request failed: {exception.Message}");
        }
    }

    private sealed record ProviderSendRequest(string To, string Body);
    private sealed record ProviderSendResponse(string Id);
}
