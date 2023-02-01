using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.UI
{
    public struct PositionStyle
    {
        public float Pixel;
        public float Percent;

        // ...

        public PositionStyle()
        {
            Pixel = 0f;
            Percent = 0f;
        }

        // ...

        public void SetValue(float pixel, float percent)
        {
            Pixel = pixel;
            Percent = percent;
        }

        public void SetPixel(float pixel)
            => Pixel = pixel;

        public void SetPercent(float percent)
            => Percent = percent;

        public float GetPixelBaseParent(float pixel)
            => Percent * pixel + Pixel;
    }
}