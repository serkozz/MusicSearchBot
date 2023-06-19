static class ChartModule
{
    private const string BASE_URL = "https://rur.hitmotop.com/artists";

    /// ----- PARSING CONSTANTS -----
    private const string ARTIST_XPATH = ".//span[@class='album-title']";
    public static OneOf<List<TrackInfo>, ErrorInfo> GetTracksChart(ushort capacity = 10)
    {
        Edge edge = new();
        OneOf<List<TrackInfo>, ErrorInfo> chartOrError = edge.GetTracksChart(capacity);

        if (chartOrError.IsT1)
            return chartOrError.AsT1;

        return chartOrError.AsT0;
    }
    public static async Task<OneOf<List<string>, ErrorInfo>> GetArtistsChartAsync(ushort capacity = 10)
    {
        try
        {
            using HttpClient client = new();
            string html = await client.GetStringAsync(BASE_URL);
            HtmlDocument document = new();
            document.LoadHtml(html);
            HtmlNodeCollection artistsNodes = document.DocumentNode.SelectNodes(ARTIST_XPATH);
            if (artistsNodes is null)
                return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить чарт исполнителей", showErrorToUser: true);
            List<string> artists = new();
            foreach (var artist in artistsNodes)
                artists.Add(artist.InnerText);
            return artists;
        }
        catch (System.Exception ex)
        {
            return new ErrorInfo(ErrorCode.ParseError, $"Возникло исключение при получении чарта исполнителей", showErrorToUser: false, ex);
        }
    }
}