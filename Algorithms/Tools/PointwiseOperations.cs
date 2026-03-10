using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        public static Image<Gray, byte> ApplyLUT(Image<Gray, byte> inputImage, byte[] lut)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
                for (int x = 0; x < inputImage.Width; x++)
                    result.Data[y, x, 0] = lut[inputImage.Data[y, x, 0]];
            return result;
        }

        public static Image<Bgr, byte> ApplyLUTColor(Image<Bgr, byte> inputImage, byte[] lut)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = lut[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = lut[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = lut[inputImage.Data[y, x, 2]];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            int width = inputImage.Width;

            for (int y = 0; y < inputImage.Height; y++)
                for (int x = 0; x < width; x++)
                {
                    result.Data[y, width - 1 - x, 0] = inputImage.Data[y, x, 0];
                    result.Data[y, width - 1 - x, 1] = inputImage.Data[y, x, 1];
                    result.Data[y, width - 1 - x, 2] = inputImage.Data[y, x, 2];
                }

            return result;
        }

        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; y++)
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                    result.Data[x, inputImage.Height - 1 - y, 1] = inputImage.Data[y, x, 1];
                    result.Data[x, inputImage.Height - 1 - y, 2] = inputImage.Data[y, x, 2];
                }

            return result;
        }

        public static Image<Bgr, byte> RotateAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; y++)
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                    result.Data[inputImage.Width - 1 - x, y, 1] = inputImage.Data[y, x, 1];
                    result.Data[inputImage.Width - 1 - x, y, 2] = inputImage.Data[y, x, 2];
                }

            return result;
        }

        public static (Image<Bgr, byte> cropped, double mean, double stdDev) CropWithStats(
            Image<Bgr, byte> inputImage, int x1, int y1, int x2, int y2)
        {
            int startX = Math.Max(0, Math.Min(x1, x2));
            int startY = Math.Max(0, Math.Min(y1, y2));
            int endX = Math.Min(inputImage.Width, Math.Max(x1, x2));
            int endY = Math.Min(inputImage.Height, Math.Max(y1, y2));

            int width = endX - startX;
            int height = endY - startY;

            Image<Bgr, byte> result = new Image<Bgr, byte>(width, height);
            double sum = 0;
            long total = (long)width * height * 3;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    for (int c = 0; c < 3; c++)
                    {
                        byte val = inputImage.Data[startY + y, startX + x, c];
                        result.Data[y, x, c] = val;
                        sum += val;
                    }

            double mean = sum / total;
            double sumSq = 0;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    for (int c = 0; c < 3; c++)
                    {
                        double diff = result.Data[y, x, c] - mean;
                        sumSq += diff * diff;
                    }

            double stdDev = Math.Sqrt(sumSq / total);
            return (result, mean, stdDev);
        }

        public static Image<Gray, byte> BrightnessContrast(Image<Gray, byte> inputImage, double a, double b)
        {
            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double value = a * i + b;
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                lut[i] = (byte)(value + 0.5);
            }
            return ApplyLUT(inputImage, lut);
        }

        public static Image<Bgr, byte> BrightnessContrast(Image<Bgr, byte> inputImage, double a, double b)
        {
            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double value = a * i + b;
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                lut[i] = (byte)(value + 0.5);
            }
            return ApplyLUTColor(inputImage, lut);
        }

        public static Image<Gray, byte> ConstantStretching(Image<Gray, byte> img)
        {
            if (img == null) return null;

            byte lmin = 255;
            byte lmax = 0;

            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                {
                    byte v = img.Data[y, x, 0];
                    if (v < lmin) lmin = v;
                    if (v > lmax) lmax = v;
                }

            if (lmax == lmin) return img.Copy();

            byte[] lut = new byte[256];
            double denom = lmax - lmin;

            for (int r = 0; r < 256; r++)
            {
                double s = (r - lmin) * 255.0 / denom;
                if (s < 0) s = 0;
                if (s > 255) s = 255;
                lut[r] = (byte)(s + 0.5);
            }

            return ApplyLUT(img, lut);
        }

        public static Image<Bgr, byte> ConstantStretching(Image<Bgr, byte> img)
        {
            if (img == null) return null;

            byte lmin = 255;
            byte lmax = 0;

            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                {
                    byte b = img.Data[y, x, 0];
                    byte g = img.Data[y, x, 1];
                    byte r = img.Data[y, x, 2];
                    int yref = (299 * r + 587 * g + 114 * b + 500) / 1000;
                    byte v = (byte)yref;
                    if (v < lmin) lmin = v;
                    if (v > lmax) lmax = v;
                }

            if (lmax == lmin) return img.Copy();

            byte[] lut = new byte[256];
            double denom = lmax - lmin;

            for (int i = 0; i < 256; i++)
            {
                double s = (i - lmin) * 255.0 / denom;
                if (s < 0) s = 0;
                if (s > 255) s = 255;
                lut[i] = (byte)(s + 0.5);
            }

            return ApplyLUTColor(img, lut);
        }
    }
}