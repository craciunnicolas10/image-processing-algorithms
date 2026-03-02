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
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte c = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = lut[c];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ApplyLUTColor(Image<Bgr, byte> inputImage, byte[] lut)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte b = inputImage.Data[y, x, 0];
                    byte g = inputImage.Data[y, x, 1];
                    byte r = inputImage.Data[y, x, 2];

                    result.Data[y, x, 0] = lut[b];
                    result.Data[y, x, 1] = lut[g];
                    result.Data[y, x, 2] = lut[r];
                }
            }

            return result;
        }

        public static Image<Gray, byte> ConstantStretching(Image<Gray, byte> img)
        {
            if (img == null) return null;

            byte lmin = 255;
            byte lmax = 0;

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    byte v = img.Data[y, x, 0];
                    if (v < lmin) lmin = v;
                    if (v > lmax) lmax = v;
                }
            }

            if (lmax == lmin) return img.Copy();

            byte[] lut = new byte[256];
            double denom = (double)(lmax - lmin);

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
            {
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
            }

            if (lmax == lmin) return img.Copy();

            byte[] lut = new byte[256];
            double denom = (double)(lmax - lmin);

            for (int i = 0; i < 256; i++)
            {
                double s = (i - lmin) * 255.0 / denom;
                if (s < 0) s = 0;
                if (s > 255) s = 255;
                lut[i] = (byte)(s + 0.5);
            }

            return ApplyLUTColor(img, lut);
        }

        #region BrightnessContrast

        public static Image<Gray, byte> BrightnessContrast(Image<Gray, byte> inputImage, double a, double b)
        {
            byte[] lut = new byte[256];
            int i = 0;

            for (; i < (255 - b) / a; i++)
            {
                double value = a * i + b + 0.5;
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                lut[i] = (byte)value;
            }

            for (; i < 256; i++)
                lut[i] = 255;

            return ApplyLUT(inputImage, lut);
        }

        public static Image<Bgr, byte> BrightnessContrast(Image<Bgr, byte> inputImage, double a, double b)
        {
            byte[] lut = new byte[256];
            int i = 0;

            for (; i < (255 - b) / a; i++)
            {
                double value = a * i + b + 0.5;
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                lut[i] = (byte)value;
            }

            for (; i < 256; i++)
                lut[i] = 255;

            return ApplyLUTColor(inputImage, lut);
        }

        #endregion
    }
}