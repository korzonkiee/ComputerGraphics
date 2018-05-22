using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAliasing
{
    public class SinglyLinkedList
    {
        public struct Edge
        {
            public int Y_Max;
            public int X;
            public int Change;
        }

        public class Node
        {
            // link to next Node in list
            public Node Next = null;
            // value of this Node
            public Edge Edge;
        }

        private Node root = null;
        private Node lastLast = null;

        public Node Last
        {
            get
            {
                Node curr = root;
                if (curr == null)
                    return null;
                while (curr.Next != null)
                {
                    if (curr.Next.Next == null)
                        lastLast = curr;
                    curr = curr.Next;
                }

                return curr;
            }
        }

        public void Append(Edge edge)
        {
            Node n = new Node { Edge = edge};
            if (root == null)
                root = n;
            else
            {
                if (Last.Edge.X >= edge.X)
                {

                }
                Last.Next = n;
            }
        }
    }
}
