
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

    public OneOf<List<TrackInfo>, ErrorInfo> GetTracksChart(ushort capacity = 100)
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

    public OneOf<List<TrackInfo>, ErrorInfo> GetNewelties(ushort capacity = 100)
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
}