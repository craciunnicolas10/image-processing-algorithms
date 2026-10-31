using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class GeometricTransformations
    {
        private static double LinearInterpolation(double frac, double f0, double f1)
        {
            return frac * f1 + (1.0 - frac) * f0;
        }

        private static double BilinearInterpolation(double fracX, double fracY, double[] f)
        {
            double p0 = LinearInterpolation(fracX, f[0], f[1]);
            double p1 = LinearInterpolation(fracX, f[2], f[3]);
            return LinearInterpolation(fracY, p0, p1);
        }

        public static Image<Gray, byte> ScaleGrayscale(Image<Gray, byte> inputImage, double sx, double sy)
        {
            int newWidth = (int)(inputImage.Width * sx);
            int newHeight = (int)(inputImage.Height * sy);
            Image<Gray, byte> result = new Image<Gray, byte>(newWidth, newHeight);

            for (int yPrime = 0; yPrime < newHeight; yPrime++)
            {
                for (int xPrime = 0; xPrime < newWidth; xPrime++)
                {
                    double xc = xPrime / sx;
                    double yc = yPrime / sy;

                    int x0 = (int)Math.Floor(xc);
                    int y0 = (int)Math.Floor(yc);

                    if (x0 >= 0 && x0 < inputImage.Width && y0 >= 0 && y0 < inputImage.Height)
                    {
                        // On the last row/column there is no right/bottom neighbour, so reuse the edge pixel
                        int x1 = Math.Min(x0 + 1, inputImage.Width - 1);
                        int y1 = Math.Min(y0 + 1, inputImage.Height - 1);
                        double fracX = xc - x0;
                        double fracY = yc - y0;

                        double[] f = new double[4];
                        f[0] = inputImage.Data[y0, x0, 0];
                        f[1] = inputImage.Data[y0, x1, 0];
                        f[2] = inputImage.Data[y1, x0, 0];
                        f[3] = inputImage.Data[y1, x1, 0];

                        double val = BilinearInterpolation(fracX, fracY, f);
                        result.Data[yPrime, xPrime, 0] = (byte)(val + 0.5);
                    }
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ScaleColor(Image<Bgr, byte> inputImage, double sx, double sy)
        {
            int newWidth = (int)(inputImage.Width * sx);
            int newHeight = (int)(inputImage.Height * sy);
            Image<Bgr, byte> result = new Image<Bgr, byte>(newWidth, newHeight);

            for (int yPrime = 0; yPrime < newHeight; yPrime++)
            {
                for (int xPrime = 0; xPrime < newWidth; xPrime++)
                {
                    double xc = xPrime / sx;
                    double yc = yPrime / sy;

                    int x0 = (int)Math.Floor(xc);
                    int y0 = (int)Math.Floor(yc);

                    if (x0 >= 0 && x0 < inputImage.Width && y0 >= 0 && y0 < inputImage.Height)
                    {
                        // On the last row/column there is no right/bottom neighbour, so reuse the edge pixel
                        int x1 = Math.Min(x0 + 1, inputImage.Width - 1);
                        int y1 = Math.Min(y0 + 1, inputImage.Height - 1);
                        double fracX = xc - x0;
                        double fracY = yc - y0;

                        for (int c = 0; c < 3; c++)
                        {
                            double[] f = new double[4];
                            f[0] = inputImage.Data[y0, x0, c];
                            f[1] = inputImage.Data[y0, x1, c];
                            f[2] = inputImage.Data[y1, x0, c];
                            f[3] = inputImage.Data[y1, x1, c];

                            double val = BilinearInterpolation(fracX, fracY, f);
                            result.Data[yPrime, xPrime, c] = (byte)(val + 0.5);
                        }
                    }
                }
            }

            return result;
        }
    }
}