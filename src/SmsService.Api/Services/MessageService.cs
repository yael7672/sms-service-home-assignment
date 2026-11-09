using Microsoft.EntityFrameworkCore;
using SmsService.Api.Data;
using SmsService.Api.Models;
using SmsService.Api.Providers;

namespace SmsService.Api.Services;

public sealed class MessageService(SmsDbContext dbContext, ISmsProvider smsProvider) : IMessageService
{
    public async Task<SmsMessage> SendAsync(
        string to,
        string body,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var message = new SmsMessage
        {
            Id = Guid.NewGuid(),
            To = to,
            Body = body,
            Status = MessageStatus.Pending,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);

        var providerResult = await smsProvider.SendAsync(to, body, cancellationToken);
        message.ProviderMessageId = providerResult.ProviderMessageId;
        message.FailureReason = providerResult.ErrorMessage;
        message.Status = providerResult.IsSuccess ? MessageStatus.Sent : MessageStatus.Failed;
        message.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return message;
    }

    public Task<SmsMessage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Messages
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<SmsMessage>> GetAllAsync(
        MessageStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Messages.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
