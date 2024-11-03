using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace SquaredleSolver;

public class Grid(char[] letters, int columns, ILocator letterBoxes)
{
    public char this[int row, int column] => letters[row * columns + column];

    public int Columns => columns;
    public int Rows => letters.Length / columns;

    public ISet<char> GetDistinctLetters() => letters.ToHashSet();

    public async Task TypeWordAsync(string word)
    {
        await letterBoxes.PressSequentiallyAsync(word);
        await letterBoxes.PressAsync("Enter");
    }

    public void Print(TextWriter writer)
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var index = r * columns + c;
                var letter = letters[index];
                writer.Write(letter);
            }

            writer.WriteLine();
        }
    }

    public static async Task<Grid> LoadAsync(IPage page)
    {
        var letterBoxes = page.Locator(".primaryContent #game .letterBoxes");
        var gridTemplateColumns = await letterBoxes.GetStylePropertyValueAsync("grid-template-columns");
        var columns = Regex.Split(gridTemplateColumns, @"\s").Length;
        var letterContainers = await letterBoxes.Locator(".letterContainer").AllAsync();
        var letters = await Task.WhenAll(letterContainers.Select(container => container.ReadLetterAsync()));
        return new Grid(letters, columns, letterBoxes);
    }
}
