public class TrackInfo
{
    public string TrackName { get; }
    public string TrackArtist { get; }
    public string TrackAlbum { get; }
    public string ThumbnailUrl { get; }
    public TrackDetails? TrackDetails { get; set; } = default;
    public string TrackDetailsUrl { get; }

    public TrackInfo(string trackName, string trackArtist, string trackAlbum = "", string trackDetailsUrl = "", string thumbnailUrl = "")
    {
        TrackName = trackName;
        TrackArtist = trackArtist;
        TrackAlbum = trackAlbum;
        TrackDetailsUrl = trackDetailsUrl;
        ThumbnailUrl = thumbnailUrl;
    }

    public string ToShortString() => $"Название трека: {TrackName}\nИсполнитель: {TrackArtist}";

    public override string ToString()
    {
        return $"Название трека: {TrackName} "
        + Environment.NewLine +
        $"Исполнитель: {TrackArtist}"
        + Environment.NewLine +
        $"Альбом: {TrackAlbum}"
        + Environment.NewLine
        + Environment.NewLine +
        $"Детальная информация о треке:"
        + Environment.NewLine
        + Environment.NewLine +
        $"Длина: {TrackDetails?.Length} сек"
        + Environment.NewLine +
        $"Темп: {TrackDetails?.Tempo}"
        + Environment.NewLine +
        $"Тональность: {TrackDetails?.Mood}"
        + Environment.NewLine +
        $"Громкость: {TrackDetails?.Volume}"
        + Environment.NewLine +
        $"Популярность: {TrackDetails?.Popularity}%"
        + Environment.NewLine +
        $"Танцевальность: {TrackDetails?.Danceability}%"
        + Environment.NewLine +
        $"Энергичность: {TrackDetails?.Energy}%"
        + Environment.NewLine +
        $"Позитивность: {TrackDetails?.Positivity}%";

        // Decomment to apply info to user

        // + Environment.NewLine +
        // $"Речитость: {TrackDetails?.Speech}%"
        // + Environment.NewLine +
        // $"Живость: {TrackDetails?.Vitality}%";
        // + Environment.NewLine +
        // $"Инструментальность: {TrackDetails?.Instrumentality}%";
    }
}