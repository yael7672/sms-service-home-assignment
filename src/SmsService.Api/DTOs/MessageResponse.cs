using SmsService.Api.Models;

namespace SmsService.Api.DTOs;

public sealed record MessageResponse(
    Guid Id,
    string To,
    string Body,
    MessageStatus Status,
    string? ProviderMessageId,
    string? FailureReason,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
