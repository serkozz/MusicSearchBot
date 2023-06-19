class Program
{
    static void Main()
    {
        var botClient = new TelegramBotClient(BotConfigConstants.TOKEN);
        using CancellationTokenSource cts = new();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = new UpdateType[] {
            UpdateType.Message,
            UpdateType.InlineQuery,
            UpdateType.CallbackQuery,
        },
            ThrowPendingUpdates = true
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );
        Console.WriteLine("Super Music Bot Started!");
        Console.ReadLine();
        cts.Cancel();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
    {
        try
        {
            if (update.Message is null)
                return;
            await TextProcessor.Process(client, update.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ExceptionInfo: {0}", ex);
        }
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
