using Microsoft.AspNetCore.Mvc;
using SmsService.Api.DTOs;
using SmsService.Api.Models;
using SmsService.Api.Services;

namespace SmsService.Api.Controllers;

[ApiController]
[Route("messages")]
public sealed class MessagesController(IMessageService messageService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateMessageResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateMessageResponse>> Send(
        SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var message = await messageService.SendAsync(
            request.To,
            request.Body.Trim(),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = message.Id },
            new CreateMessageResponse(message.Id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<MessageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var message = await messageService.GetByIdAsync(id, cancellationToken);
        return message is null ? NotFound() : Ok(ToResponse(message));
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MessageResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MessageResponse>>> GetAll(
        [FromQuery] MessageStatus? status,
        CancellationToken cancellationToken)
    {
        var messages = await messageService.GetAllAsync(status, cancellationToken);
        return Ok(messages.Select(ToResponse).ToList());
    }

    private static MessageResponse ToResponse(SmsMessage message) => new(
        message.Id,
        message.To,
        message.Body,
        message.Status,
        message.ProviderMessageId,
        message.FailureReason,
        message.CreatedAtUtc,
        message.UpdatedAtUtc);
}
