using Dithering.ColorQuantization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Dithering.ImageProcessors
{
    public class OctreeQuantImageProcessor : SingleThreadImageProcessor
    {
        private readonly Octree octree;
        private List<ColorQuantization.Color> palette;

        public int PaletteSize { get; set; } = 8;

        private bool buildOctree = false;
        private bool quantize = false;

        public OctreeQuantImageProcessor()
        {
            this.octree = new Octree();
        }

        public override void Process(Bitmap bitmap)
        {
            buildOctree = true; quantize = false;
            base.Process(bitmap);

            palette = octree.MakePalette(PaletteSize);

            buildOctree = false; quantize = true;
            base.Process(bitmap);

            octree.Clear();
        }

        protected override unsafe void ProcessPixel(byte* r, byte* g, byte* b, int yPos, int xPos)
        {

            var color = new ColorQuantization.Color()
            {
                R = *r,
                G = *g,
                B = *b
            };

            if (buildOctree)
                octree.Add(color);

            if (quantize)
            {

                int idx = octree.GetPaletteIndex(color);

                *r = palette[idx].R;
                *g = palette[idx].G;
                *b = palette[idx].B;
            }
        }
    }
}
