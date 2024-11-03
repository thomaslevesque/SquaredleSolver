using Microsoft.Playwright;

namespace SquaredleSolver;

public static class LocatorExtensions
{
    public static Task<string> GetStylePropertyValueAsync(this ILocator locator, string propertyName)
    {
        return locator.EvaluateAsync<string>(
            $"element => window.getComputedStyle(element).getPropertyValue('{propertyName}')");
    }

    public static Task<bool> HasClassAsync(this ILocator locator, string className)
    {
        return locator.EvaluateAsync<bool>($"element => element.classList.contains('{className}')");
    }

    public static async Task<char> ReadLetterAsync(this ILocator letterContainer)
    {
        var isBlank = await letterContainer.HasClassAsync("blank");
        if (isBlank)
            return ' ';
        var letter = await letterContainer.Locator(".letter .content .unnecessaryWrapper").InnerTextAsync();
        return letter.Length > 0 ? letter[0] : ' ';
    }

    public static async Task TryCloseExplainerAsync(this IPage page)
    {
        var locators = await page.Locator("#explainer:not([hidden]) #explainerClose").AllAsync();
        foreach (var locator in locators)
        {
            await locator.ClickAsync();
        }
    }
    public static async Task TryClosePopupAsync(this IPage page)
    {
        var locators = await page.Locator(".popup:not([hidden]) .closeBtn").AllAsync();
        foreach (var locator in locators)
        {
            await locator.ClickAsync();
        }
    }
}
