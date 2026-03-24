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

        public static double[,] GaussMask(double sigma = 1.0, int size = 5)
        {
            double[,] mask = new double[size, size];
            int offset = size / 2;
            double sum = 0.0;

            for (int y = -offset; y <= offset; y++)
            {
                for (int x = -offset; x <= offset; x++)
                {
                    double exponent = -((x * x + y * y) / (2.0 * sigma * sigma));
                    double value = (1.0 / (2.0 * Math.PI * sigma * sigma)) * Math.Exp(exponent);
                    mask[y + offset, x + offset] = value;
                    sum += value;
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    mask[i, j] /= sum;
                }
            }

            return mask;
        }

        public static Image<Gray, byte> UnsharpMask(Image<Gray, byte> inputImage)
        {
            double[,] mask = GaussMask(1.0, 5);
            Image<Gray, byte> lowPass = ApplyFilter(inputImage, mask);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int f = inputImage.Data[y, x, 0];
                    int f_low = lowPass.Data[y, x, 0];
                    int f_sharp = 2 * f - f_low;

                    if (f_sharp < 0) f_sharp = 0;
                    if (f_sharp > 255) f_sharp = 255;

                    result.Data[y, x, 0] = (byte)f_sharp;
                }
            }

            return result;
        }

        public static Image<Bgr, byte> UnsharpMask(Image<Bgr, byte> inputImage)
        {
            double[,] mask = GaussMask(1.0, 5);
            Image<Bgr, byte> lowPass = ApplyFilter(inputImage, mask);
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        int f = inputImage.Data[y, x, c];
                        int f_low = lowPass.Data[y, x, c];
                        int f_sharp = 2 * f - f_low;

                        if (f_sharp < 0) f_sharp = 0;
                        if (f_sharp > 255) f_sharp = 255;

                        result.Data[y, x, c] = (byte)f_sharp;
                    }
                }
            }

            return result;
        }
    }
}