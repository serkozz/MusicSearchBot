public static class TrackInfoModule
{
    private const string SEARCH_QUERY = "search?q=";
    private const string BASE_URL = "https://musicstax.com/ru/";

    /// ----- TRACK PARSING CONSTANTS -----
    private const string SONG_CARD_XPATH = ".//*[@class='song-details search-song-details']";
    private const string SONG_THUMBNAIL_XPATH = ".//a[@class='song-image search-image']/img";
    private const string SONG_ARTIST_XPATH = ".//*[@class='song-artist search-artist']";
    private const string SONG_NAME_XPATH = ".//*[@class='song-title search-title']//u";
    private const string SONG_ALBUM_XPATH = ".//*[@class='song-album']";
    private const string SONG_FULL_DATA_XPATH = ".//*[@class='song-details-right search-details-right']";

    /// ----- TRACK DETAILS PARSING CONSTANTS -----
    private const string SONG_DETAILS_PROPERTY_XPATH = ".//*[@class='song-fact-container-stat' or @class='song-bar-statistic-number']";

    public static async Task<OneOf<TrackInfo, ErrorInfo>> GetTrackInfoAsync(string query)
    {
        query = query.Unidecode();
        OneOf<TrackInfo, ErrorInfo> trackInfoOrError = await ParseTrackInfoAsync(query);

        if (trackInfoOrError.IsT1)
            return trackInfoOrError.AsT1;

        OneOf<TrackDetails, ErrorInfo> trackDetailsOrError = await ParseTrackDetailsAsync(trackInfoOrError.AsT0);

        if (trackDetailsOrError.IsT1)
            return trackDetailsOrError.AsT1;

        trackInfoOrError.AsT0.TrackDetails = trackDetailsOrError.AsT0;
        return trackInfoOrError.AsT0;
    }

    private static async Task<OneOf<TrackInfo, ErrorInfo>> ParseTrackInfoAsync(string queryString)
    {
        try
        {
            using HttpClient client = new(new HttpClientHandler
            {
                // Если запрос возвращает код 3--, то осуществляется запрос по headers.location благодаря AllowAutoRedirect
                AllowAutoRedirect = true
            });
            string searchUrl = BASE_URL + SEARCH_QUERY + Utility.PrepareUrl(queryString, true);
            HttpResponseMessage response = await client.GetAsync(searchUrl);
            if (!response.IsSuccessStatusCode)
                return new ErrorInfo(ErrorCode.ParseError, $"Запрос, совершенный по URL: {searchUrl}, вернул статус код: {response.StatusCode}");

            string Html = await response.Content.ReadAsStringAsync();

            if (String.IsNullOrEmpty(Html))
                return new ErrorInfo(ErrorCode.ParseError, $"Полученная страница не имеет содержимого");

            HtmlDocument document = new();
            document.LoadHtml(Html);

            var parseErrors = document.ParseErrors.ToList();
            // if (parseErrors.Count != 0)
            // return new ErrorInfo(ErrorCode.ParseError, $"Полученная страница не была преобразована в DOM", showErrorToUser: false, parseErrors);

            HtmlNode trackNode = document.DocumentNode.SelectSingleNode(SONG_CARD_XPATH);

            if (trackNode is null)
                return new ErrorInfo(ErrorCode.ParseError, $"Трек не найден! (Запрос: {queryString})", showErrorToUser: true);

            string artist = trackNode.SelectSingleNode(SONG_ARTIST_XPATH).InnerText.TrimStart().TrimEnd();
            string name = trackNode.SelectSingleNode(SONG_NAME_XPATH).InnerText.TrimStart().TrimEnd();
            string album = trackNode.SelectSingleNode(SONG_ALBUM_XPATH).InnerText.TrimStart().TrimEnd();
            string url = BASE_URL + trackNode.SelectSingleNode(SONG_FULL_DATA_XPATH).Attributes["href"].Value.TrimStart().TrimEnd().Remove(0, 4);
            string thumbnailUrl = trackNode.SelectSingleNode(SONG_THUMBNAIL_XPATH).Attributes["src"].Value;
            return new TrackInfo(name, artist, album, url, thumbnailUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ErrorInfo(ErrorCode.ParseError, $"Возникло исключение при получении информации о треке", showErrorToUser: false, ex);
        }
    }

    public static async Task<OneOf<TrackDetails, ErrorInfo>> ParseTrackDetailsAsync(TrackInfo trackInfo)
    {
        try
        {
            using HttpClient client = new(new HttpClientHandler
            {
                // Если запрос возвращает код 3--, то осуществляется запрос по headers.location благодаря AllowAutoRedirect
                AllowAutoRedirect = true
            });
            HttpResponseMessage response = await client.GetAsync(trackInfo.TrackDetailsUrl);

            if (!response.IsSuccessStatusCode)
                return new ErrorInfo(ErrorCode.ParseError, $"Запрос, совершенный по URL: {trackInfo.TrackDetailsUrl}, вернул статус код: {response.StatusCode}");

            string Html = await response.Content.ReadAsStringAsync();

            if (String.IsNullOrEmpty(Html))
                return new ErrorInfo(ErrorCode.ParseError, $"Полученная страница не имеет содержимого");

            HtmlDocument document = new();
            document.LoadHtml(Html);

            HtmlNodeCollection trackFactsNodes = document.DocumentNode.SelectNodes(SONG_DETAILS_PROPERTY_XPATH);
            int index = 0;

            int tempo = 0; int popularity = 0; int danceability = 0; int energy = 0; int positivity = 0; int speech = 0; int vitality = 0; int instrumentality = 0;
            string length = string.Empty; string mood = string.Empty; string volume = string.Empty;

            foreach (HtmlNode factNode in trackFactsNodes)
            {
                switch (index)
                {
                    case 0:
                        length = factNode.InnerText.Trim();
                        break;
                    case 1:
                        tempo = Int32.Parse(factNode.InnerText.Trim());
                        break;
                    case 2:
                        mood = factNode.InnerText.Trim();
                        break;
                    case 3:
                        volume = factNode.InnerText.Trim();
                        break;
                    case 4:
                        popularity = Int32.Parse(factNode.InnerText.Trim().TrimEnd('%'));
                        break;
                    case 5:
                        danceability = Int32.Parse(factNode.InnerText.Trim().TrimEnd('%'));
                        break;
                    case 6:
                        energy = Int32.Parse(factNode.InnerText.Trim().TrimEnd('%'));
                        break;
                    case 7:
                        positivity = Int32.Parse(factNode.InnerText.Trim().TrimEnd('%'));
                        break;
                    case 8:
                        speech = Int32.Parse(factNode.InnerText.Trim().TrimEnd('%'));
                        break;
                    case 9:
                        vitality = Int32.Parse(factNode.InnerText.Trim().TrimEnd('%'));
                        break;
                    case 10:
                        instrumentality = Int32.Parse(factNode.InnerText.Trim().TrimEnd('%'));
                        break;
                }
                index++;
            }
            return new TrackDetails(length, tempo, mood, volume, popularity, danceability, energy, positivity, speech, vitality, instrumentality);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ErrorInfo(ErrorCode.ParseError, $"Возникло исключение при получении информации о треке", showErrorToUser: false, ex);
        }
    }
}