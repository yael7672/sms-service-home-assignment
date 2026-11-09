using SmsService.Api.Models;

namespace SmsService.Api.Services;

public interface IMessageService
{
    Task<SmsMessage> SendAsync(string to, string body, CancellationToken cancellationToken = default);
    Task<SmsMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SmsMessage>> GetAllAsync(
        MessageStatus? status,
        CancellationToken cancellationToken = default);
}
