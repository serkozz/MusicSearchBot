static class TextProcessor
{
    private const string ERROR_MESSAGE = "Возникла непредвиденная ошибка, обратитесь к создателю бота!";
    public static async Task Process(ITelegramBotClient client, Message message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        if (string.IsNullOrWhiteSpace(message.Text))
            return;

        if (message.Text.Contains("/"))
        {
            await ProcessCommand(client, message);
            return;
        }

        await client.SendTextMessageAsync(message.Chat.Id, BotReplyConstants.UNKNOWN_COMMAND);
    }

    public static async Task ProcessCommand(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
            return;

        string[] splittedCommand = message.Text.Split(" ");

        switch (splittedCommand[0])
        {
            case "/help":
                await client.SendTextMessageAsync(message.Chat.Id, BotReplyConstants.COMMANDS_INFO);
                return;
            case "/start":
                await client.SendTextMessageAsync(message.Chat.Id, BotReplyConstants.GREETINGS_MESSAGE);
                return;
            case "/search":
                // TODO: Расширенный поиск
                var extendedSearchTracksOrError = TrackInfoModule.ExtendedSearch(string.Join(" ", splittedCommand[1..splittedCommand.Length]));
                if (extendedSearchTracksOrError.IsT0)
                {
                    List<TrackInfo> extendedSearchTracks = extendedSearchTracksOrError.AsT0.Take(10).ToList();
                    string resultString = extendedSearchTracks.ListToString(toShortString: false);
                    await client.SendTextMessageAsync(message.Chat.Id, $"Найдено: {extendedSearchTracks.Count} треков\n\n{resultString}");
                    return;
                }
                await client.SendTextMessageAsync(message.Chat.Id, extendedSearchTracksOrError.AsT1.ShowErrorToUser ? extendedSearchTracksOrError.AsT1.Message : ERROR_MESSAGE);
                return;
            case "/info":
                var trackInfoOrError = await TrackInfoModule.GetTrackInfoAsync(string.Join(" ", splittedCommand[1..splittedCommand.Length]));
                if (trackInfoOrError.IsT0)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, $"Информация о треке:\n{trackInfoOrError.AsT0}");
                    return;
                }
                await client.SendTextMessageAsync(message.Chat.Id, trackInfoOrError.AsT1.ShowErrorToUser ? trackInfoOrError.AsT1.Message : ERROR_MESSAGE);
                return;
            case "/lyrics":
                var lyricsOrError = TrackLyricsModule.GetTrackLyrics(string.Join(" ", splittedCommand[1..splittedCommand.Length]));
                if (lyricsOrError.IsT0)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, $"Текст трека: \n {lyricsOrError.AsT0}");
                    return;
                }
                await client.SendTextMessageAsync(message.Chat.Id, lyricsOrError.AsT1.ShowErrorToUser ? lyricsOrError.AsT1.Message : ERROR_MESSAGE);
                return;
            case "/lyricsPart":
                var tracksOrError = TrackLyricsModule.GetTracksByLyrics(string.Join(" ", splittedCommand[1..splittedCommand.Length]));
                if (tracksOrError.IsT0)
                {
                    List<TrackInfo> searchResult = tracksOrError.AsT0;
                    string resultString = searchResult.ListToString();
                    await client.SendTextMessageAsync(message.Chat.Id, $"Совпадений: {searchResult.Count} треков\n\n{resultString}");
                    return;
                }
                await client.SendTextMessageAsync(message.Chat.Id, tracksOrError.AsT1.ShowErrorToUser ? tracksOrError.AsT1.Message : ERROR_MESSAGE);
                return;
            case "/download":
                var streamOrError = await TrackDownloadModule.DownloadTrackAsync(string.Join(" ", splittedCommand[1..splittedCommand.Length]));
                if (streamOrError.IsT0)
                {
                    await client.SendAudioAsync(message.Chat.Id, InputFile.FromStream(streamOrError.AsT0));
                    return;
                }
                await client.SendTextMessageAsync(message.Chat.Id, streamOrError.AsT1.ShowErrorToUser ? streamOrError.AsT1.Message : ERROR_MESSAGE);
                return;
            case "/chart":
                OneOf<List<TrackInfo>, ErrorInfo> chartOrError;
                if (splittedCommand.Length == 2 && UInt16.TryParse(splittedCommand[1], out ushort capacity))
                    chartOrError = ChartModule.GetTracksChart(capacity > 50 ? (ushort)50 : capacity);
                else
                    chartOrError = ChartModule.GetTracksChart();
                if (chartOrError.IsT1)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, chartOrError.AsT1.ShowErrorToUser ? chartOrError.AsT1.Message : ERROR_MESSAGE);
                    return;
                }
                List<TrackInfo> chartTracks = chartOrError.AsT0;
                string result = chartTracks.ListToString();
                await client.SendTextMessageAsync(message.Chat.Id, $"Чарт получен: {chartTracks.Count} треков\n\n{result}");
                return;
            case "/artistsChart":
                OneOf<List<string>, ErrorInfo> artistsOrError;
                if (splittedCommand.Length == 2 && UInt16.TryParse(splittedCommand[1], out capacity))
                    artistsOrError = await ChartModule.GetArtistsChartAsync(capacity > 50 ? (ushort)50 : capacity);
                else
                    artistsOrError = await ChartModule.GetArtistsChartAsync();
                if (artistsOrError.IsT1)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, artistsOrError.AsT1.ShowErrorToUser ? artistsOrError.AsT1.Message : ERROR_MESSAGE);
                    return;
                }
                List<string> chartArtists = artistsOrError.AsT0;
                result = chartArtists.ListToString();
                await client.SendTextMessageAsync(message.Chat.Id, $"Топ исполнителей: {chartArtists.Count} исполнителей\n\n{result}");
                return;
            case "/artist":
                OneOf<List<TrackInfo>, ErrorInfo> artistTracksOrError;
                // if (UInt16.TryParse(splittedCommand[1], out capacity))
                //     artistTracksOrError = ChartModule.GetArtistTopTracks(, capacity > 50 ? (ushort)50 : (ushort)50);
                // else
                artistTracksOrError = ChartModule.GetArtistTopTracks(string.Join(" ", splittedCommand[1..splittedCommand.Length]), 50);
                if (artistTracksOrError.IsT1)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, artistTracksOrError.AsT1.ShowErrorToUser ? artistTracksOrError.AsT1.Message : ERROR_MESSAGE);
                    return;
                }
                List<TrackInfo> artistTracks = artistTracksOrError.AsT0;
                result = artistTracks.ListToString();
                await client.SendTextMessageAsync(message.Chat.Id, $"Треки исполнителя {artistTracks[0].TrackArtist}: {artistTracks.Count} треков\n\n{result}");
                return;
            case "/new":
                OneOf<List<TrackInfo>, ErrorInfo> neweltiesOrError;
                if (splittedCommand.Length == 2 && UInt16.TryParse(splittedCommand[1], out capacity))
                    neweltiesOrError = TrackNeweltiesModule.GetNewelties(capacity > 50 ? (ushort)50 : capacity);
                else
                    neweltiesOrError = ChartModule.GetTracksChart();
                if (neweltiesOrError.IsT1)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, neweltiesOrError.AsT1.ShowErrorToUser ? neweltiesOrError.AsT1.Message : ERROR_MESSAGE);
                    return;
                }
                List<TrackInfo> neweltiesTracks = neweltiesOrError.AsT0;
                result = neweltiesTracks.ListToString();
                await client.SendTextMessageAsync(message.Chat.Id, $"Новинки получены: {neweltiesTracks.Count} треков\n\n{result}");
                return;
            default:
                await client.SendTextMessageAsync(message.Chat.Id, BotReplyConstants.UNKNOWN_COMMAND);
                return;
        }
    }
}