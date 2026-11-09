namespace SmsService.Api.Models;

public sealed class SmsMessage
{
    public Guid Id { get; set; }
    public string To { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public MessageStatus Status { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
