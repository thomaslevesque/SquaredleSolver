using System.CommandLine;
using Microsoft.Playwright;
using SquaredleSolver;

var urlOption = new Option<string>(
    ["--url", "-u"],
    () => "https://squaredle.app/",
    "URL of the puzzle (default to the daily Squaredle)");

var browserNameOption = new Option<string?>(
    ["--browser-name", "-b"],
    () => null,
    "Browser name (chromium, firefox or webkit)");

var browserPathOption = new Option<string?>(
    ["--browser-path", "-p"],
    () => null,
    "Browser path (defaults to the bundled browser)");

var rootCommand = new RootCommand("Solves Squaredle puzzles")
{
    urlOption,
    browserNameOption,
    browserPathOption,
};

rootCommand.SetHandler(async (url, browserName, browserPath) =>
{
    using var playwright = await Playwright.CreateAsync();
    var browserType = GetBrowserType(playwright, browserName);
    var launchOptions = GetLaunchOptions(browserType, browserPath);
    await using var browser = await browserType.LaunchAsync(launchOptions);

    var page = await browser.NewPageAsync();
    await page.GotoAsync(url);

    await page.Locator("a.skipTutorial").ClickAsync();
    await page.Locator("#confirmAccept").ClickAsync();

    var grid = await Grid.LoadAsync(page);
    grid.Print(Console.Out);
    Console.WriteLine("**********");

    var availableLetters = grid.GetDistinctLetters();
    var words = await WordDictionary.LoadAsync("words.txt", availableLetters);
    var solver = new Solver(grid, words);
    foreach (var word in solver.EnumerateWords())
    {
        Console.WriteLine(word);
        await page.TryClosePopupAsync();
        await page.TryCloseExplainerAsync();
        await grid.TypeWordAsync(word);
    }

    if (launchOptions.Headless is false)
    {
        Console.WriteLine("Press enter to close the browser");
        Console.ReadLine();
    }
}, urlOption, browserNameOption, browserPathOption);

rootCommand.Invoke(args);

static IBrowserType GetBrowserType(IPlaywright playwright, string? browserName)
{
    return browserName switch
    {
        "chromium" => playwright.Chromium,
        "firefox" => playwright.Firefox,
        "webkit" => playwright.Webkit,
        "" => playwright.Chromium,
        null => playwright.Chromium,
        var s => throw new NotSupportedException($"Unsupported browser '{s}'")
    };
}

static BrowserTypeLaunchOptions GetLaunchOptions(IBrowserType browser, string? browserPath)
{
    var options = new BrowserTypeLaunchOptions();
    if (!string.IsNullOrEmpty(browserPath))
    {
        options.ExecutablePath = browserPath;
    }
    else if (!File.Exists(browser.ExecutablePath))
    {
        var installedPath = GetInstalledBrowserPath(browser.Name);
        if (!string.IsNullOrEmpty(installedPath))
        {
            options.ExecutablePath = installedPath;
        }
        else
        {
            throw new Exception("The specified browser is not installed");
        }
    }

    options.Headless = false;
    return options;
}

static string? GetInstalledBrowserPath(string browser)
{
    throw new NotImplementedException();
}
