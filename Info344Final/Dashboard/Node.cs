using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard
{
    public class Node
    {
        public Dictionary<char, Node> Edges = new Dictionary<char, Node>();
        public bool IsTerminal { get; set; }
    }
}
