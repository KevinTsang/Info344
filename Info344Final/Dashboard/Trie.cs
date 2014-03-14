using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard
{
    public class Trie
    {
        private Node Root;

        public Trie()
        {
            Root = new Node();
        }

        public Trie(string[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (Root == null)
                    Root = new Node();
                InsertIntoTrie(word);
            }
        }

        public Node getRoot()
        {
            return Root;
        }

        public void InsertIntoTrie(string word)
        {
            Node node = Root;
            for (int i = 0; i < word.Length; i++)
            {
                char letter = word[i];
                Node next;
                if (!node.Edges.TryGetValue(letter, out next))
                {
                    next = new Node();
                    node.Edges.Add(letter, next);
                }
                node = next;
            }
            node.IsTerminal = true;
        }

        private Node GetStartingNode(string prefix, Node element)
        {
            foreach (char c in prefix)
            {
                Node subnode;
                if (element.Edges.TryGetValue(c, out subnode))
                    element = subnode;
                else return null;
            }
            return element;
        }

        private List<string> trieTraversal(Stack<Node> s, List<string> suggestions, string word)
        {
            if (suggestions.Count == 10)
                return suggestions;
            Node element = s.Pop();
            if (element.IsTerminal)
            {
                suggestions.Add(word);
                if (suggestions.Count == 10)
                    return suggestions;
            }
            foreach (char c in element.Edges.Keys)
            {
                Node adjacent;
                if (element.Edges.TryGetValue(c, out adjacent))
                {
                    s.Push(adjacent);
                    trieTraversal(s, suggestions, word + c);
                }
            }

            if (s.Count > 0)
                return trieTraversal(s, suggestions, word);
            else return suggestions;
        }

        public List<string> SearchPhrasesForPrefix(string prefix)
        {
            Stack<Node> s = new Stack<Node>();
            List<string> suggestions = new List<string>();
            Node subtree = GetStartingNode(prefix, Root);
            if (subtree != null)
            {
                s.Push(subtree);
                suggestions = trieTraversal(s, suggestions, "");
                for (int i = 0; i < suggestions.Count; i++)
                {
                    suggestions[i] = prefix + suggestions[i];
                }
            }
            else suggestions.Add("No results found.");
            return suggestions;
        }
    }
}
