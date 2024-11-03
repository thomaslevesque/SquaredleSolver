namespace SquaredleSolver;

    public class Trie
    {
        private Dictionary<char, Trie>? _children;

        public bool HasValue { get; private set; }

        public bool IsEmpty() => !HasValue && (_children is null || _children.Count == 0);

        public bool ContainsPrefix(string text)
        {
            return GetDescendantNode(text) is not null;
        }

        public bool ContainsWord(string text)
        {
            return GetDescendantNode(text) is { HasValue: true };
        }

        public void Add(string text)
        {
            Add(text, 0);
        }

        private void Add(string text, int position)
        {
            if (position >= text.Length)
            {
                this.HasValue = true;
                return;
            }

            char letter = text[position];
            var currentNode = GetChildNode(letter, true)!;
            currentNode.Add(text, position + 1);
        }

        public Trie? GetDescendantNode(string prefix)
        {
            var current = this;
            foreach (var c in prefix)
            {
                current = current.GetChildNode(c, false);
                if (current is null)
                    return null;
            }

            return current;
        }

        public Trie? GetChildNode(char letter) => GetChildNode(letter, false);

        private Trie? GetChildNode(char letter, bool add)
        {
            if (_children == null)
            {
                if (!add)
                    return null;

                _children = new Dictionary<char, Trie>();
            }

            if (!_children.TryGetValue(letter, out var childNode) && add)
            {
                childNode = new Trie();
                _children[letter] = childNode;
            }
            return childNode;
        }

        public (int Nodes, int Words) GetCount()
        {
            int nodes = 1;
            int words = HasValue ? 1 : 0;

            if (_children is null)
                return (nodes, words);


            foreach (var child in _children)
            {
                var (childNodes, childWords) = child.Value.GetCount();
                nodes += childNodes;
                words += childWords;
            }

            return (nodes, words);
        }
    }
