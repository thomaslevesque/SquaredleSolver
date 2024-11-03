using System.Collections.Immutable;

namespace SquaredleSolver;

public class Solver(Grid grid, Trie words)
{
    public IEnumerable<string> EnumerateWords()
    {
        var q =
            from row in Enumerable.Range(0, grid.Rows)
            from col in Enumerable.Range(0, grid.Columns)
            from w in EnumerateWords(row, col)
            select w;
        return q.Distinct();
    }

    private IEnumerable<string> EnumerateWords(int startRow, int startCol)
    {
        var initialPosition = new Position(startRow, startCol);
        var initialState = State.Start(initialPosition, grid[startRow, startCol], words);
        var queue = new Queue<State>();
        queue.Enqueue(initialState);
        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            if (state.CurrentValue.Length > 3 && state.CandidateWords?.HasValue is true)
                yield return state.CurrentValue;
            var neighbors = state.CurrentPosition.GetNeighbors()
                .Where(n => n.IsValid(grid) && !state.UsedPositions.Contains(n));
            foreach (var n in neighbors)
            {
                var letter = grid[n.Row, n.Column];
                var newState = state.Next(n, letter);
                if (newState.CandidateWords?.IsEmpty() ?? true)
                    continue;

                queue.Enqueue(newState);
            }
        }
    }

    private readonly record struct Position(int Row, int Column)
    {
        public IEnumerable<Position> GetNeighbors()
        {
            yield return new(Row - 1, Column - 1);
            yield return new(Row - 1, Column);
            yield return new(Row - 1, Column + 1);
            yield return new(Row, Column - 1);
            yield return new(Row, Column + 1);
            yield return new(Row + 1, Column - 1);
            yield return new(Row + 1, Column);
            yield return new(Row + 1, Column + 1);
        }

        public bool IsValid(Grid grid) => Row >= 0 && Row < grid.Rows && Column >= 0 && Column < grid.Columns;
    }

    private record State(string CurrentValue, Position CurrentPosition, IImmutableSet<Position> UsedPositions, Trie? CandidateWords)
    {
        public static State Start(Position startPosition, char startLetter, Trie words)
        {
            var prefix = startLetter.ToString();
            return new(
                prefix,
                startPosition,
                ImmutableHashSet<Position>.Empty.Add(startPosition),
                words.GetDescendantNode(prefix));
        }

        public State Next(Position newPosition, char newLetter)
        {
            var newPrefix = CurrentValue + newLetter;
            return new(
                newPrefix,
                newPosition,
                UsedPositions.Add(newPosition),
                CandidateWords?.GetChildNode(newLetter));
        }
    }
}
