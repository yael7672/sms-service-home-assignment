using System.ComponentModel.DataAnnotations;
using SmsService.Api.DTOs;

namespace SmsService.Tests;

public sealed class SendMessageRequestValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("021234567")]
    [InlineData("05012345678")]
    public void InvalidPhoneNumber_FailsValidation(string phoneNumber)
    {
        var request = new SendMessageRequest { To = phoneNumber, Body = "hello" };

        var results = Validate(request);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(SendMessageRequest.To)));
    }

    [Fact]
    public void MessageLongerThan500Characters_FailsValidation()
    {
        var request = new SendMessageRequest
        {
            To = "0501234567",
            Body = new string('a', 501)
        };

        var results = Validate(request);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(SendMessageRequest.Body)));
    }

    [Fact]
    public void WhitespaceOnlyMessage_FailsValidation()
    {
        var request = new SendMessageRequest { To = "0501234567", Body = "   " };

        var results = Validate(request);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(SendMessageRequest.Body)));
    }

    private static List<ValidationResult> Validate(object value)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), results, true);
        return results;
    }
}
