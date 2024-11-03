using Microsoft.Playwright;

namespace SquaredleSolver;

public record Letter(char Value, ILocator? Locator, int Row, int Column, bool IsBlank)
{
    public override string ToString() => Value.ToString();

    public static async Task<Letter> LoadAsync(ILocator letterContainer, int row, int column)
    {
        var isBlank = await letterContainer.HasClassAsync("blank");
        var letter = isBlank ? ' ' : await ReadLetterAsync(letterContainer);
        return new Letter(letter, letterContainer, row, column, isBlank);
    }

    private static async Task<char> ReadLetterAsync(ILocator letterContainer)
    {
        var text = await letterContainer.Locator(".letter .content .unnecessaryWrapper").InnerTextAsync();
        return text.Length > 0 ? text[0] : ' ';
    }
}
