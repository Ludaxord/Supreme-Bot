using System;
using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Supreme_Bot
{
    public static class BitmapExtensions
    {

        public static bool Contains(this Bitmap template, Bitmap bmp)
        {
            const Int32 divisor = 4;
            const Int32 epsilon = 10;

            ExhaustiveTemplateMatching etm = new ExhaustiveTemplateMatching(0.9f);

            TemplateMatch[] tm = etm.ProcessImage(
                new ResizeNearestNeighbor(template.Width / divisor, template.Height / divisor).Apply(template),
                new ResizeNearestNeighbor(bmp.Width / divisor, bmp.Height / divisor).Apply(bmp)
                );

            if (tm.Length == 1)
            {
                Rectangle tempRect = tm[0].Rectangle;

                if (Math.Abs(bmp.Width / divisor - tempRect.Width) < epsilon
                    &&
                    Math.Abs(bmp.Height / divisor - tempRect.Height) < epsilon)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
