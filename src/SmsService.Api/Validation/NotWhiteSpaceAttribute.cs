using System.ComponentModel.DataAnnotations;

namespace SmsService.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotWhiteSpaceAttribute : ValidationAttribute
{
    public NotWhiteSpaceAttribute()
        : base("The {0} field cannot contain only whitespace.")
    {
    }

    public override bool IsValid(object? value) =>
        value is not string text || !string.IsNullOrWhiteSpace(text);
}
