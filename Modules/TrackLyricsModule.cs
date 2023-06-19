public static class TrackLyricsModule
{
    public static OneOf<string, ErrorInfo> GetTrackLyrics(string query)
    {
        Edge edge = new();
        string lyrics = edge.GetLyrics(query);

        if (string.IsNullOrEmpty(lyrics))
            return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить текст трека: {query}", true);
        return lyrics;
    }

    public static OneOf<List<TrackInfo>, ErrorInfo> GetTracksByLyrics(string lyrics)
    {
        Edge edge = new();
        OneOf<List<TrackInfo>, ErrorInfo> tracksOrError = edge.GetTrackInfoByLyrics(lyrics);
        if (tracksOrError.IsT1)
            return tracksOrError.AsT1;

        var tracks = tracksOrError.AsT0;

        if (tracks.Count == 0)
            return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить треки, содержащие следующий текст: {lyrics}", true);
        return tracks;
    }
}
