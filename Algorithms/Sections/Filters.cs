using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(Image<TColor, TDepth> image, double[,] filter)
            where TColor : struct, IColor
            where TDepth : new()
        {
            int w = filter.GetLength(1) / 2;
            int h = filter.GetLength(0) / 2;
            Image<TColor, TDepth> borderedImage = new Image<TColor, TDepth>(image.Width + 2 * w, image.Height + 2 * h);
            CvInvoke.CopyMakeBorder(image, borderedImage, h, h, w, w, Emgu.CV.CvEnum.BorderType.Reflect101, new Emgu.CV.Structure.MCvScalar(0));
            Image<TColor, TDepth> result = new Image<TColor, TDepth>(image.Size);
            bool isByte = typeof(TDepth) == typeof(byte);
            int channels = image.NumberOfChannels;
            for (int y = h; y < borderedImage.Height - h; y++)
                for (int x = w; x < borderedImage.Width - w; x++)
                    for (int c = 0; c < channels; c++)
                    {
                        double sum = 0;
                        for (int i = -h; i <= h; i++)
                            for (int j = -w; j <= w; j++)
                                sum += Convert.ToDouble(borderedImage.Data[y + i, x + j, c]) * filter[i + h, j + w];
                        if (isByte)
                        {
                            double val = Math.Max(Math.Min(sum + 0.5, 255), 0);
                            result.Data[y - h, x - w, c] = (TDepth)Convert.ChangeType(val, typeof(TDepth));
                        }
                        else
                            result.Data[y - h, x - w, c] = (TDepth)Convert.ChangeType(sum, typeof(TDepth));
                    }
            return result;
        }
    }
}