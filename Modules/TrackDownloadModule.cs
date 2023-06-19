public static class TrackDownloadModule
{
    private const string SEARCH_QUERY = "search?q=";
    private const string BASE_URL = "https://rur.hitmotop.com/";

    private const string DOWNLOAD_SONG_XPATH = ".//a[@class='track__download-btn']";

    public static async Task<OneOf<Stream, ErrorInfo>> DownloadTrackAsync(string query)
    {
        try
        {
            using HttpClient client = new();
            var downloadUriOrError = await GetDownloadUri(query, client);
            if (downloadUriOrError.IsT1)
                return downloadUriOrError.AsT1;
            return await client.GetStreamAsync(downloadUriOrError.AsT0);
        }
        catch (System.Exception ex)
        {
            return new ErrorInfo(ErrorCode.ParseError, $"Возникло исключение при получении трека", showErrorToUser: false, ex);
        }
    }

    private static async Task<OneOf<string, ErrorInfo>> GetDownloadUri(string query, HttpClient client)
    {
        try
        {
            string html = await client.GetStringAsync(BASE_URL + SEARCH_QUERY + Utility.PrepareUrl(query, true));
            HtmlDocument document = new();
            document.LoadHtml(html);
            HtmlNode trackNode = document.DocumentNode.SelectSingleNode(DOWNLOAD_SONG_XPATH);
            if (trackNode is null)
                return new ErrorInfo(ErrorCode.ParseError, $"Трек не найден", showErrorToUser: true);
            return trackNode.Attributes["href"].Value;
        }
        catch (System.Exception ex)
        {
            return new ErrorInfo(ErrorCode.ParseError, $"Возникло исключение при получении трека", showErrorToUser: false, ex);
        }
    }
}
