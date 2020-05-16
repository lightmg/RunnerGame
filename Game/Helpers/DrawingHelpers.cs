using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace Game.Helpers
{
    public static class DrawingHelpers
    {
        public static Image CreateSquare(int width, int height, Color color)
        {
            var image = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(image);
            graphics.Clear(color);
            return image;
        }

        public static Image AddBorders(this Image image, Color color)
        {
            using var graphics = Graphics.FromImage(image);
            graphics.DrawRectangle(new Pen(color, 3), -1, 0, image.Width, image.Height);
            return image;
        }

        public static IEnumerable<Image> ExtractImageFrames(this Image image)
        {
            var rawFrames = image.IsMultipleFrames()
                ? Enumerable.Range(0, image.GetFrameCount(FrameDimension.Time))
                    .Select(i =>
                    {
                        image.SelectActiveFrame(FrameDimension.Time, i);
                        return (Image) image.Clone();
                    })
                    .AsEnumerable()
                : new[] {image};

            return rawFrames;
        }

        //Honestly stolen from https://stackoverflow.com/a/29785775 instead of a lot of slow Bitmap.GetPixel and SetPixel opertaions
        public static Bitmap AutoCrop(this Bitmap bmp, Color backgroundColor)
        {
            if (Image.GetPixelFormatSize(bmp.PixelFormat) != 32)
                throw new InvalidOperationException($"Pixel format {bmp.PixelFormat} is not supported");

            // Set the left crop point to the width so that the logic below
            // will set the left value to the first non crop color pixel it comes across.
            var left = bmp.Width;
            // Set the top crop point to the height so that the logic below
            // will set the top value to the first non crop color pixel it comes across.
            var top = bmp.Height;
            var bottom = 0;
            var right = 0;

            var bmpData = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);

            unsafe
            {
                var dataPtr = (byte*) bmpData.Scan0;

                for (var y = 0; y < bmp.Height; y++)
                {
                    for (var x = 0; x < bmp.Width; x++)
                    {
                        var rgbPtr = dataPtr + x * 4;

                        var b = rgbPtr[0];
                        var g = rgbPtr[1];
                        var r = rgbPtr[2];
                        var a = rgbPtr[3];

                        // If any of the pixel RGBA values don't match and the crop color is not transparent,
                        // or if the crop color is transparent and the pixel A value is not transparent
                        if ((backgroundColor.A == 0 && a != 0) ||
                            (backgroundColor.A > 0 && (b != backgroundColor.B ||
                                                 g != backgroundColor.G ||
                                                 r != backgroundColor.R ||
                                                 a != backgroundColor.A)))
                        {
                            if (x < left)
                                left = x;

                            if (x >= right)
                                right = x + 1;

                            if (y < top)
                                top = y;

                            if (y >= bottom)
                                bottom = y + 1;
                        }
                    }

                    dataPtr += bmpData.Stride;
                }
            }

            bmp.UnlockBits(bmpData);

            if (left < right && top < bottom)
                return bmp.Clone(new Rectangle(left, top, right - left, bottom - top), bmp.PixelFormat);

            return bmp;
        }

        public static Bitmap Resize(this Image image, Size targetSize)
        {
            if (image.Size == targetSize)
                return new Bitmap(image);

            var tempBitmap = new Bitmap(targetSize.Width, targetSize.Height);

            using var graphics = Graphics.FromImage(tempBitmap);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, new Rectangle(0, 0, targetSize.Width, targetSize.Height), 0,
                0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            return tempBitmap;
        }

        private static bool IsMultipleFrames(this Image image)
        {
            return image.RawFormat.Guid.In(ImageFormat.Gif.Guid, ImageFormat.Tiff.Guid) &&
                   image.GetFrameCount(FrameDimension.Time) > 1;
        }
    }
}