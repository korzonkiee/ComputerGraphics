using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dithering.ColorQuantization
{
    public class Octree
    {
        private const int MAX_DEPTH = 8;

        public class OctreeNode
        {
            public ulong Sum_R { get; private set; }
            public ulong Sum_G { get; private set; }
            public ulong Sum_B { get; private set; }

            public Color GetColor()
            {
                return new Color()
                {
                    R = (byte)(Sum_R / Refs),
                    G = (byte)(Sum_G / Refs),
                    B = (byte)(Sum_B / Refs),
                };
            }

            public ulong Refs { get; private set; }
            public bool IsLeaf => Refs > 0;
            public int Depth { get; private set; }

            public int PaletteIndex { get; set; } = 0;

            private readonly Octree octree;

            public OctreeNode(Octree octree, int depth)
            {
                this.octree = octree;

                Depth = depth;

                // Hence we have access to children of a node
                // we need only 0 -> 7 levels. 8th is accessed
                // from 7th level. 0 level is root.
                if (Depth < MAX_DEPTH)
                    AddSelfToOctreeLevelsNodes(depth);
            }

            public OctreeNode[] Children { get; private set; }
                = new OctreeNode[8];

            public int CheckReduce()
            {
                int removedChildren = 0;

                for (int i = 0; i < 8; i++)
                {
                    if (Children[i] != null)
                    {
                        removedChildren += 1;
                    }
                }

                return removedChildren;
            }

            public int Reduce(int numberOfLeaves, int desiredColors)
            {
                int removedChildren = 0;

                for (int i = 0; i < 8; i++)
                {
                    if (Children[i] != null)
                    {
                        this.Sum_R += Children[i].Sum_R;
                        this.Sum_G += Children[i].Sum_G;
                        this.Sum_B += Children[i].Sum_B;
                        this.Refs += Children[i].Refs;

                        removedChildren += 1;

                        Children[i] = null;
                    }
                }

                return removedChildren;
            }

            public void AddColor(Octree octree, Color color, int depth)
            {
                if (depth >= MAX_DEPTH)
                {
                    this.Sum_R += color.R;
                    this.Sum_G += color.G;
                    this.Sum_B += color.B;
                    this.Refs = this.Refs + 1;

                    return;
                }

                int colorIdx = GetColorIndex(color, depth);

                if (Children[colorIdx] == null)
                    Children[colorIdx] = new OctreeNode(octree, depth + 1);

                Children[colorIdx].AddColor(octree, color, depth + 1);
            }

            private int GetColorIndex(Color color, int depth)
            {
                int mask = 0x80 >> depth;
                int index = 0;

                if ((mask & color.R) > 0)
                    index |= 4;
                if ((mask & color.G) > 0)
                    index |= 2;
                if ((mask & color.B) > 0)
                    index |= 1;

                return index;
            }

            public int GetPaletteIndex(Color color, int depth)
            {
                if (IsLeaf)
                    return PaletteIndex;

                int colorIdx = GetColorIndex(color, depth);
                if (Children[colorIdx] != null)
                    return Children[colorIdx].GetPaletteIndex(color, depth + 1);

                for (int i = 0; i < 8; i++)
                {
                    if (Children[i] != null)
                        return Children[i].GetPaletteIndex(color, depth + 1);
                }

                return 0;
            }

            private void AddSelfToOctreeLevelsNodes(int depth)
            {
                octree.LevelsNodes[depth].Add(this);
            }
        }

        OctreeNode root;

        private List<OctreeNode> Leafs = new List<OctreeNode>();
        private List<List<OctreeNode>> LevelsNodes = new List<List<OctreeNode>>();

        public Octree()
        {
            for (int i = 0; i < MAX_DEPTH; i++)
                LevelsNodes.Add(new List<OctreeNode>());

            root = new OctreeNode(this, depth: 0);
        }

        public void Clear()
        {
            Leafs.Clear();
            foreach (var levelNodes in LevelsNodes)
            {
                levelNodes.Clear();
            }

            root = new OctreeNode(this, depth: 0);
        }

        public void Add(Color color)
        {
            root.AddColor(this, color, 0);
        }

        public int GetPaletteIndex(Color color)
        {
            return root.GetPaletteIndex(color, 0);
        }

        public List<Color> MakePalette(int desiredColors)
        {
            List<Color> palette = new List<Color>();

            Leafs.Clear();
            LoadLeaves(root);
            int numberOfLeaves = Leafs.Count;

            for (int i = MAX_DEPTH - 1; i >= 0; i--)
            {
                for (int j = 0; j < LevelsNodes[i].Count; j++)
                {
                    var node = LevelsNodes[i][j];

                    int checkRemovedNoted = node.CheckReduce();
                    if (numberOfLeaves - checkRemovedNoted + 1 <= desiredColors)
                        break;

                    int removedNodes = node.Reduce(numberOfLeaves, desiredColors);

                    // +1 Because we have removed x leaves but
                    // now current node becomes leaf.
                    numberOfLeaves = numberOfLeaves - removedNodes + 1;
                    if (numberOfLeaves <= desiredColors)
                        break;
                }

                if (numberOfLeaves <= desiredColors)
                    break;
            }

            Leafs.Clear();
            LoadLeaves(root);

            int paletteIndex = 0;
            foreach (var leaf in Leafs)
            {
                if (paletteIndex >= desiredColors)
                    break;
                
                palette.Add(leaf.GetColor());
                leaf.PaletteIndex = paletteIndex;
                paletteIndex++;
            }

            return palette;
        }

        private void LoadLeaves(OctreeNode node)
        {
            for (int i = 0; i < 8; i++)
            {
                OctreeNode child = node?.Children[i];
                if (child != null)
                {
                    if (child.IsLeaf)
                        Leafs.Add(child);
                    else
                        LoadLeaves(child);
                }
            }
        }

        public List<Color> GetKMostPopularColors(int k)
        {
            var mostPopularColors = Leafs
                .OrderByDescending(n => n.Refs)
                .Take(k).Select(n => n.GetColor());

            foreach (var leaf in Leafs)
            {
                double minDist = double.MaxValue;
                int minDistColorIdx = 0;

                int index = 0;
                foreach (var color in mostPopularColors)
                {
                    double dist = leaf.GetColor().DistanceTo(color);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minDistColorIdx = index;
                    }

                    index++;
                }

                leaf.PaletteIndex = index;
            }

            return mostPopularColors.ToList();
        }
    }
}
