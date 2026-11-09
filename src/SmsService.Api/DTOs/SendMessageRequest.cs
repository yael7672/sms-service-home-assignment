using System.ComponentModel.DataAnnotations;
using SmsService.Api.Validation;

namespace SmsService.Api.DTOs;

public sealed class SendMessageRequest
{
    [Required]
    [RegularExpression(@"^05\d{8}$", ErrorMessage = "Phone number must be an Israeli mobile number in the format 05XXXXXXXX.")]
    public string To { get; init; } = string.Empty;

    [Required]
    [NotWhiteSpace]
    [StringLength(500, MinimumLength = 1)]
    public string Body { get; init; } = string.Empty;
}
