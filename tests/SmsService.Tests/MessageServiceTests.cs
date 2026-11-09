using Microsoft.EntityFrameworkCore;
using SmsService.Api.Data;
using SmsService.Api.Models;
using SmsService.Api.Providers;
using SmsService.Api.Services;

namespace SmsService.Tests;

public sealed class MessageServiceTests
{
    [Fact]
    public async Task SendAsync_WhenProviderSucceeds_SavesSentMessage()
    {
        await using var db = CreateDatabase();
        var service = new MessageService(db, new FakeProvider(ProviderSendResult.Success("provider-1")));

        var result = await service.SendAsync("0501234567", "hello");

        Assert.Equal(MessageStatus.Sent, result.Status);
        Assert.Equal("provider-1", result.ProviderMessageId);
        Assert.Null(result.FailureReason);
        Assert.Equal(1, await db.Messages.CountAsync());
    }

    [Fact]
    public async Task SendAsync_WhenProviderFails_SavesFailedMessageAndReason()
    {
        await using var db = CreateDatabase();
        var service = new MessageService(db, new FakeProvider(ProviderSendResult.Failure("provider unavailable")));

        var result = await service.SendAsync("0501234567", "hello");

        Assert.Equal(MessageStatus.Failed, result.Status);
        Assert.Equal("provider unavailable", result.FailureReason);
    }

    [Fact]
    public async Task GetAllAsync_WithStatus_ReturnsOnlyMatchingMessages()
    {
        await using var db = CreateDatabase();
        db.Messages.AddRange(
            CreateMessage(MessageStatus.Sent),
            CreateMessage(MessageStatus.Failed),
            CreateMessage(MessageStatus.Failed));
        await db.SaveChangesAsync();
        var service = new MessageService(db, new FakeProvider(ProviderSendResult.Success("unused")));

        var result = await service.GetAllAsync(MessageStatus.Failed);

        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.Equal(MessageStatus.Failed, x.Status));
    }

    private static SmsDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<SmsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SmsDbContext(options);
    }

    private static SmsMessage CreateMessage(MessageStatus status) => new()
    {
        Id = Guid.NewGuid(),
        To = "0501234567",
        Body = "test",
        Status = status,
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    };

    private sealed class FakeProvider(ProviderSendResult result) : ISmsProvider
    {
        public Task<ProviderSendResult> SendAsync(
            string to,
            string body,
            CancellationToken cancellationToken = default) => Task.FromResult(result);
    }
}
