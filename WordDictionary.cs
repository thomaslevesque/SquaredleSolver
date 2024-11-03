namespace SquaredleSolver;

public static class WordDictionary
{
    public static async Task<Trie> LoadAsync(string path, ISet<char> availableLetters)
    {
        var words = File.ReadLinesAsync(path)
            .Select(s => s.ToUpperInvariant())
            .Where(word => IsValidWord(word, availableLetters));

        var trie = new Trie();
        await foreach (var word in words)
        {
            trie.Add(word);
        }

        return trie;
    }

    private static bool IsValidWord(string word, ISet<char> availableLetters) =>
        word.Length > 3 &&
        word.All(c => char.IsLetter(c) && availableLetters.Contains(c));
}
