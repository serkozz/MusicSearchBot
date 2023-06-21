public struct ExtendedSearchInfo
{
    public List<Genre> Genres { get; set; }
    public BPMInfo BPMInfo { get; set; }
    public Mood Mood { get; set; }

    public ExtendedSearchInfo()
    {
        Genres = new List<Genre>();
    }

    public ExtendedSearchInfo(List<Genre> genres, BPMInfo bpmInfo, Mood mood)
    {
        Genres = genres;
        BPMInfo = bpmInfo;
        Mood = mood;
    }

}

public enum Genre
{
    acoustic,
    afrobeat,
    alt_rock,
    alternative,
    ambient,
    anime,
    black_metal,
    bluegrass,
    blues,
    bossanova,
    brazil,
    breakbeat,
    british,
    cantopop,
    chicago_house,
    children,
    chill,
    classical,
    club,
    comedy,
    country,
    dance,
    dancehall,
    death_metal,
    deep_house,
    detroit_techno,
    disco,
    disney,
    drum_and_bass,
    dub,
    dubstep,
    edm,
    electro,
    electronic,
    emo,
    folk,
    forro,
    french,
    funk,
    garage,
    german,
    gospel,
    goth,
    grindcore,
    groove,
    grunge,
    guitar,
    happy,
    hard_rock,
    hardcore,
    hardstyle,
    heavy_metal,
    hip_hop,
    holidays,
    honky_tonk,
    house,
    idm,
    indian,
    indie,
    indie_pop,
    industrial,
    iranian,
    j_dance,
    j_idol,
    j_pop,
    j_rock,
    jazz,
    k_pop,
    kids,
    latin,
    latino,
    malay,
    mandopop,
    metal,
    metal_misc,
    metalcore,
    minimal_techno,
    movies,
    mpb,
    new_age,
    new_release,
    opera,
    pagode,
    party,
    philipines_opm,
    piano,
    pop,
    pop_film,
    post_dubstep,
    power_pop,
    progressive_house,
    psych_rock,
    punk,
    punk_rock,
    r_n_b,
    rainy_day,
    reggae,
    reggaeton,
    road_trip,
    rock,
    rock_n_roll,
    rockabilly,
    romance,
    sad,
    salsa,
    samba,
    sertanejo,
    show_tunes,
    singer_songwriter,
    ska,
    sleep,
    songwriter,
    soul,
    soundtracks,
    spanish,
    study,
    summer,
    swedish,
    synth_pop,
    tango,
    techno,
    trance,
    trip_hop,
    turkish,
    work_out,
    world_music
}

public enum Mood
{
    Any,
    AnyMaj,
    AnyMin,
    C,
    CMaj,
    CMin,
    CSharp,
    CSharpMaj,
    CSharpMin,
    D,
    DMaj,
    DMin,
    DSharp,
    DSharpMaj,
    DSharpMin,
    E,
    EMaj,
    EMin,
    F,
    FMaj,
    FMin,
    FSharp,
    FSharpMaj,
    FSharpMin,
    G,
    GMaj,
    GMin,
    GSharp,
    GSharpMaj,
    GSharpMin,
    A,
    AMaj,
    AMin,
    ASharp,
    ASharpMaj,
    ASharpMin,
    B,
    BMaj,
    BMin
}

public struct BPMInfo
{
    private short _min;
    public short Min
    {
        get { return _min; }
        set
        {
            if (value <= 30) _min = 30;
            else if (value >= 200) _min = 200;
            else _min = value;
            Validate();
        }
    }
    private short _max;
    public short Max
    {
        get { return _max; }
        set
        {
            if (value <= 30) _max = 30;
            else if (value >= 200) _max = 200;
            else _max = value;
            Validate();
        }
    }
    private void Validate()
    {
        if (Min > Max)
            Max = Min;
    }

    public BPMInfo(short min, short max)
    {
        Min = min;
        Max = max;
    }
}