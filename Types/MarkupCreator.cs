public static class MarkupCreator
{
    public static IReplyMarkup CreateStartMarkup()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new [] // first row
            {
                InlineKeyboardButton.WithCallbackData("🔍 Поиск", BotConfigConstants.SEARCH_CALLBACK_DATA),
                InlineKeyboardButton.WithCallbackData("🔎 Расширенный поиск", BotConfigConstants.EXPANDED_SEARCH_CALLBACK_DATA),
            },
            new [] // second row
            {
                InlineKeyboardButton.WithCallbackData("📃 Поиск по отрывку", BotConfigConstants.LYRICS_SEARCH_CALLBACK_DATA),
                InlineKeyboardButton.WithCallbackData("📊 Чарт треков", BotConfigConstants.CHART_CALLBACK_DATA),
            },
            new [] // third row
            {
                InlineKeyboardButton.WithCallbackData("❤️ Топ исполнителей", BotConfigConstants.TOP_ARTISTS_CALLBACK_DATA),
                InlineKeyboardButton.WithCallbackData("💍 Новинки", BotConfigConstants.NOVELTIES_CALLBACK_DATA),
            }
        });
    }
}