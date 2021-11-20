/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Falak
{
    public class Node: IEnumerable<Node> {

        public IList<Node> children = new List<Node>();

        public Node this[int index] {
            get {
                return children[index];
            }
        }
        
        public Token AnchorToken { get; set; }

        public void Add(Node node) {
            children.Add(node);
        }

        public IEnumerator<Node> GetEnumerator() {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator
            System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return $"{GetType().Name} {AnchorToken}";
        }

        public string ToStringTree() {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }

        public int childrenLength(){
            int sum = 0;
			foreach (var child in this) {
                sum ++;
            }
			return sum;
        }

        static void TreeTraversal(Node node, string indent, StringBuilder sb) {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children) {
                TreeTraversal(child, indent + "  ", sb);
            }
        }
    }
}