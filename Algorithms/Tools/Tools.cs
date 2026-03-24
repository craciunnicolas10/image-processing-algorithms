using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion

        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage,byte T=128)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (inputImage.Data[y, x, 0] >= T)
                        result.Data[y, x, 0] = 255;
                    else//redundant imaginea are pixelii negri impliciti
                        result.Data[y, x, 0] = 0;
                }
            }
            return result;
        }
        #endregion
        #region Crop
        public static Image<Gray, byte> Crop(Image<Gray, byte> inputImage, int x1, int y1, int x2, int y2)
        {
            int width = x2 - x1;
            int height = y2 - y1;
            Image<Gray, byte> result = new Image<Gray, byte>(width, height);
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    result.Data[y, x, 0] = inputImage.Data[y1 + y, x1 + x, 0];
            return result;
        }

        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, int x1, int y1, int x2, int y2)
        {
            int width = x2 - x1;
            int height = y2 - y1;
            Image<Bgr, byte> result = new Image<Bgr, byte>(width, height);
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y1 + y, x1 + x, 0];
                    result.Data[y, x, 1] = inputImage.Data[y1 + y, x1 + x, 1];
                    result.Data[y, x, 2] = inputImage.Data[y1 + y, x1 + x, 2];
                }
            return result;
        }

        public static (double mean, double stdDev) ComputeMeanStdDev(Image<Gray, byte> inputImage)
        {
            double sum = 0;
            int total = inputImage.Width * inputImage.Height;
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                    sum += inputImage.Data[y, x, 0];
            double mean = sum / total;

            double sqSum = 0;
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    double diff = inputImage.Data[y, x, 0] - mean;
                    sqSum += diff * diff;
                }
            double stdDev = System.Math.Sqrt(sqSum / total);
            return (mean, stdDev);
        }
        #endregion
        #region Mirror
        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                    result.Data[y, inputImage.Width - 1 - x, 0] = inputImage.Data[y, x, 0];
            return result;
        }

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, inputImage.Width - 1 - x, 0] = inputImage.Data[y, x, 0];
                    result.Data[y, inputImage.Width - 1 - x, 1] = inputImage.Data[y, x, 1];
                    result.Data[y, inputImage.Width - 1 - x, 2] = inputImage.Data[y, x, 2];
                }
            return result;
        }
        #endregion

        #region Rotate Clockwise
        public static Image<Gray, byte> RotateClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                    result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
            return result;
        }

        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                    result.Data[x, inputImage.Height - 1 - y, 1] = inputImage.Data[y, x, 1];
                    result.Data[x, inputImage.Height - 1 - y, 2] = inputImage.Data[y, x, 2];
                }
            return result;
        }
        #endregion

        #region Rotate Anti-Clockwise
        public static Image<Gray, byte> RotateAntiClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                    result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
            return result;
        }

        public static Image<Bgr, byte> RotateAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                    result.Data[inputImage.Width - 1 - x, y, 1] = inputImage.Data[y, x, 1];
                    result.Data[inputImage.Width - 1 - x, y, 2] = inputImage.Data[y, x, 2];
                }
            return result;
        }
        #endregion
    }
}