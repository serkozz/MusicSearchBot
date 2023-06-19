static class TrackNeweltiesModule
{
    public static OneOf<List<TrackInfo>, ErrorInfo> GetNewelties(ushort capacity = 10)
    {
        Edge edge = new();
        OneOf<List<TrackInfo>, ErrorInfo> chartOrError = edge.GetNewelties(capacity);

        if (chartOrError.IsT1)
            return chartOrError.AsT1;

        return chartOrError.AsT0;
    }
}