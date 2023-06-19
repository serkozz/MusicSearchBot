public class TrackDetails
{
    /// <summary>
    /// Длительность трека в мин:сек
    /// </summary>
    /// <value></value>
    public string Length { get; } = string.Empty;
    /// <summary>
    /// Темп трека
    /// </summary>
    /// <value></value>
    public int Tempo { get; } = default;
    /// <summary>
    /// Тональность трека
    /// </summary>
    /// <value></value>
    public string Mood { get; } = string.Empty;
    /// <summary>
    /// Громкость трека
    /// </summary>
    /// <value></value>
    public string Volume { get; } = string.Empty;
    /// <summary>
    /// Популярность трека в процентах
    /// </summary>
    /// <value></value>
    public int Popularity { get; } = default;
    /// <summary>
    /// Танцевальность трека в процентах
    /// </summary>
    /// <value></value>
    public int Danceability { get; } = default;
    /// <summary>
    /// Энергичность трека в процентах
    /// </summary>
    /// <value></value>
    public int Energy { get; } = default;
    /// <summary>
    /// Позитивность трека в процентах
    /// </summary>
    /// <value></value>
    public int Positivity { get; } = default;
    /// <summary>
    /// Речитость трека в процентах
    /// </summary>
    /// <value></value>
    public int Speech { get; } = default;
    /// <summary>
    /// Живость трека в процентах
    /// </summary>
    /// <value></value>
    public int Vitality { get; } = default;
    /// <summary>
    /// Инструментальность трека в процентах
    /// </summary>
    /// <value></value>
    public int Instrumentality { get; } = default;

    public TrackDetails() { }

    public TrackDetails(string length, int tempo, string mood, string volume, int popularity, int danceability, int energy, int positivity, int speech, int vitality, int instrumentality)
    {
        Length = length;
        Tempo = tempo;
        Mood = mood;
        Volume = volume;
        Popularity = popularity;
        Danceability = danceability;
        Energy = energy;
        Positivity = positivity;
        Speech = speech;
        Vitality = vitality;
        Instrumentality = instrumentality;
    }
}