
using System.Collections.ObjectModel;

public class Edge
{
    private EdgeDriver Driver { get; set; }
    private readonly TimeSpan Timeout = new(0, 0, 2);

    public Edge()
    {
        EdgeOptions options = new()
        {
            PageLoadStrategy = PageLoadStrategy.Eager,

        };
        options.AddArgument("--headless");
        Driver = new EdgeDriver(options);
        Driver.Manage().Timeouts().ImplicitWait = Timeout;
    }

    public EdgeDriver Navigate(string url)
    {
        Driver.Navigate().GoToUrl(url);
        return Driver;
    }

    private IWebElement? FindElementByClassName(string className, byte waitSecTimeout = 7)
    {
        try
        {
            var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, waitSecTimeout));
            var element = wait.Until(driver => driver.FindElement(By.ClassName(className)));
            return element;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Element with className: '" + className + "' was not found in current context page.");
            return null;
        }
    }

    private IWebElement? FindElementBySelector(string cssSelector, byte waitSecTimeout = 7)
    {
        try
        {
            var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, waitSecTimeout));
            var element = wait.Until(driver => driver.FindElement(By.CssSelector(cssSelector)));
            return element;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Element with selector: '" + cssSelector + "' was not found in current context page.");
            return null;
        }
    }

    public string GetLyrics(string query)
    {
        try
        {
            Driver.Navigate().GoToUrl("https://genius.com");
            var searchBox = Driver.FindElement(By.Name("q"));
            ArgumentNullException.ThrowIfNull(searchBox, "searchBox");
            searchBox.SendKeys(query);
            searchBox.Submit();
            var trackElement = this.FindElementByClassName("mini_card");
            ArgumentNullException.ThrowIfNull(trackElement, "trackElement");
            trackElement.Click();
            var textBlocks = Driver.FindElements(By.XPath(".//div[@class='Lyrics__Container-sc-1ynbvzw-5 Dzxov']"));
            string result = string.Empty;
            foreach (var block in textBlocks)
            {
                result += block.Text;
            }
            Driver.Quit();
            return result;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return string.Empty;
        }
    }

    public OneOf<List<TrackInfo>, ErrorInfo> GetTrackInfoByLyrics(string lyrics)
    {
        try
        {
            List<TrackInfo> tracks = new();
            Driver.Navigate().GoToUrl("https://genius.com");
            var searchBox = Driver.FindElement(By.Name("q"));
            ArgumentNullException.ThrowIfNull(searchBox, "searchBox");
            searchBox.SendKeys(lyrics);
            searchBox.Submit();
            var showMoreBtn = this.FindElementBySelector(".full_width_button.u-bottom_margin");
            ArgumentNullException.ThrowIfNull(showMoreBtn, "trackElement");
            showMoreBtn.Click();
            var trackCards = Driver.FindElements(By.XPath(".//*[@class='u-display_block ']"));
            string result = string.Empty;
            foreach (var card in trackCards)
            {
                var infoElement = card.FindElement(By.CssSelector(".mini_card-title_and_subtitle"));
                string trackName = infoElement.FindElement(By.CssSelector(".mini_card-title")).Text;
                string trackArtist = infoElement.FindElement(By.CssSelector(".mini_card-subtitle")).Text;
                tracks.Add(new TrackInfo(trackName, trackArtist));
            }
            Driver.Quit();
            return tracks;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить ни одного трека, содержащего следующий текст: {lyrics}", true);
        }
    }

    public OneOf<List<TrackInfo>, ErrorInfo> GetVKTracks(ushort capacity = 100)
    {
        try
        {
            List<TrackInfo> tracks = new();
            var trackNodes = Driver.FindElements(By.XPath(".//div[@class='audio_row__performer_title']"));
            ArgumentNullException.ThrowIfNull(trackNodes, "trackNodes");
            foreach (var node in trackNodes)
            {
                var trackName = node.FindElement(By.CssSelector(".audio_row__title._audio_row__title")).FindElement(By.CssSelector("a")).Text;
                var trackArtists = node.FindElement(By.CssSelector(".audio_row__performers")).FindElements(By.CssSelector("a"));
                string trackArtist = string.Empty;
                foreach (var artist in trackArtists)
                {
                    trackArtist += artist.Text + ", ";
                }
                trackArtist = trackArtist.TrimEnd(',', ' ');
                tracks.Add(new TrackInfo(trackName, trackArtist));
            }
            Driver.Quit();
            return tracks.Take(capacity).ToList();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить треки", true);
        }
    }

    public OneOf<List<TrackInfo>, ErrorInfo> GetTracksChart(ushort capacity)
    {
        try
        {
            Navigate("https://vk.com/audio?block=chart");
            var chartOrError = GetVKTracks(capacity);
            return chartOrError.IsT0 ? chartOrError.AsT0 : new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить чарт", true);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить чарт", true);
        }
    }

    public OneOf<List<TrackInfo>, ErrorInfo> GetNewelties(ushort capacity)
    {
        try
        {
            Navigate("https://vk.com/audio?block=new_songs");
            var neweltiesOrError = GetVKTracks(capacity);
            return neweltiesOrError.IsT0 ? neweltiesOrError.AsT0 : new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить новинки", true);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить новинки", true);
        }
    }

    /// FIXME: Не всегда работает как надо (капча, будь она неладна)
    public OneOf<List<TrackInfo>, ErrorInfo> GetArtistTracks(string artist, ushort capacity)
    {
        try
        {
            string baseUrl = "https://music.yandex.ru/";
            string url = $"{baseUrl}search?text={artist}";
            Driver.Navigate().GoToUrl(url);
            string artistSpecificUrl = Driver.FindElement(By.XPath(".//*[@class='artist']//a")).GetAttribute("href");
            Driver.Navigate().GoToUrl(artistSpecificUrl + "/tracks");
            new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until<bool>(driver => driver.FindElement(By.XPath(".//div[@class='d-generic-page-head__main-top']//h1']")).Displayed);
            var artistName = this.Driver.FindElement(By.XPath(".//div[@class='d-generic-page-head__main-top']//h1")).Text;
            var tracksNamesElements = this.Driver.FindElements(By.CssSelector(".d-track__name"));
            string result = string.Empty;
            List<TrackInfo> tracks = new();
            foreach (var nameElement in tracksNamesElements)
                tracks.Add(new TrackInfo(nameElement.GetAttribute("title"), artistName));
            Driver.Quit();
            return tracks;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить треки указанного исполнителя", true);
        }
    }

    public OneOf<List<TrackInfo>, ErrorInfo> ExtendedSearch(ExtendedSearchInfo extendedSearchInfo)
    {
        try
        {
            string baseUrl = "https://musicstax.com/ru/search/advanced";
            Driver.Navigate().GoToUrl(baseUrl);
            IWebElement? genreSelector;
            IWebElement? moodSelector;
            ReadOnlyCollection<IWebElement?> genreSelectorOptions;
            var bpmInputs = Driver.FindElements(By.XPath(".//input[@class='advanced-search']"));
            IWebElement? minBpmInput = bpmInputs[0];
            IWebElement? maxBpmInput = bpmInputs[1];
            int index;

            foreach (var genre in extendedSearchInfo.Genres)
            {
                genreSelector = Driver.FindElement(By.CssSelector(".select2-search__field"));
                genreSelector.Click();
                genreSelectorOptions = Driver.FindElements(By.TagName("option"));
                index = (int)genre + 1;
                genreSelectorOptions[index]?.Click();
            }

            minBpmInput.SendKeys(extendedSearchInfo.BPMInfo.Min.ToString());
            maxBpmInput.SendKeys(extendedSearchInfo.BPMInfo.Max.ToString());

            moodSelector = Driver.FindElements(By.CssSelector(".select2-selection__rendered"))[1];
            moodSelector.Click();
            List<IWebElement?> moodSelectorOptions = Driver.FindElements(By.XPath(".//li"))
                .Where((el, ind) => el.GetAttribute("class").Contains("select2-results__option--selectable"))
                .ToList();

            index = (int)extendedSearchInfo.Mood;
            moodSelectorOptions[index]?.Click();

            IWebElement? submitBtn = Driver.FindElements(By.XPath(".//button")).Last();
            submitBtn.Click();

            var tracks = ParseTracksInfos(Driver.FindElements(By.XPath(".//a[@class='artist-seed-track-right']")));

            Driver.Quit();
            return tracks;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ErrorInfo(ErrorCode.ParseError, $"Невозможно получить треки", true);
        }
    }

    private List<TrackInfo> ParseTracksInfos(ReadOnlyCollection<IWebElement> trackCards)
    {
        List<TrackInfo> tracks = new();
        foreach (var card in trackCards)
        {
            var cardDetails = card.FindElements(By.TagName("div"));
            string trackArtist = cardDetails[0].Text.Trim();
            string trackName = cardDetails[1].FindElement(By.TagName("u")).Text.Trim();

            var details = cardDetails[2].Text.Split("•");
            var volumeAndPopularity = details[3].Split("\r\n\r\n", 2);

            TrackDetails trackDetails = new TrackDetails(length: details[2].Trim(),
                                                        tempo: Int32.Parse(details[0].Trim().Split(" ", 2)[0]),
                                                        mood: details[1].Trim(),
                                                        volume: volumeAndPopularity[0].Trim(),
                                                        popularity: Int32.Parse(volumeAndPopularity[1].Trim().Split(" ", 2)[0].TrimEnd('%')),
                                                        danceability: Int32.Parse(details[4].Trim().Split(" ", 2)[0].TrimEnd('%')),
                                                        energy: Int32.Parse(details[5].Trim().Split(" ", 2)[0].TrimEnd('%')),
                                                        positivity: Int32.Parse(details[7].Trim().Split(" ", 2)[0].TrimEnd('%')),
                                                        speech: default,
                                                        vitality: Int32.Parse(details[6].Split("\r\n", 2)[0].Trim().Split(" ", 2)[0].TrimEnd('%')),
                                                        instrumentality: default);

            TrackInfo trackInfo = new(trackName, trackArtist)
            {
                TrackDetails = trackDetails
            };
            tracks.Add(trackInfo);
        }
        Console.WriteLine($"Text");
        return tracks;
    }
}