using Telegram.Bot.Polling;

namespace StudentHelper.Services.Interfaces;

public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken cancellationToken);
}