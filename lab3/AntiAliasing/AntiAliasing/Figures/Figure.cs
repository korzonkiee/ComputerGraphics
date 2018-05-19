using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAliasing.Figures
{
    public abstract class Figure
    {
        public virtual void Render(BitmapData bitmapData, RenderMode mode = RenderMode.Normal)
        {
            switch (mode)
            {
                case RenderMode.Normal:
                    NormalRender(bitmapData);
                    break;
                case RenderMode.AntiAliasing:
                    AntiAliasingRender(bitmapData);
                    break;
                default:
                    break;
            }
        }

        public abstract void NormalRender(BitmapData bitmapData);
        public abstract void AntiAliasingRender(BitmapData bitmapData);
        public abstract Figure SuperSampled();
    }
}
