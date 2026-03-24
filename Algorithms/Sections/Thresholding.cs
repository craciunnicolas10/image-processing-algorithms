using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        public static Image<Gray, byte> OtsuThresholding(Image<Gray, byte> inputImage)
        {
            if (inputImage == null) return null;

            int[] histogram = new int[256];

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte value = inputImage.Data[y, x, 0];
                    histogram[value]++;
                }
            }

            int totalPixels = inputImage.Width * inputImage.Height;

            double sum = 0;
            for (int i = 0; i < 256; i++)
                sum += i * histogram[i];

            double sumBackground = 0;
            int weightBackground = 0;
            int weightForeground = 0;

            double maxVariance = 0;
            int optimalThreshold = 0;

            for (int t = 0; t < 256; t++)
            {
                weightBackground += histogram[t];
                if (weightBackground == 0)
                    continue;

                weightForeground = totalPixels - weightBackground;
                if (weightForeground == 0)
                    break;

                sumBackground += t * histogram[t];

                double meanBackground = sumBackground / weightBackground;
                double meanForeground = (sum - sumBackground) / weightForeground;

                double betweenClassVariance =
                    weightBackground * weightForeground *
                    (meanBackground - meanForeground) *
                    (meanBackground - meanForeground);

                if (betweenClassVariance > maxVariance)
                {
                    maxVariance = betweenClassVariance;
                    optimalThreshold = t;
                }
            }

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte value = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = value > optimalThreshold ? (byte)255 : (byte)0;
                }
            }

            return result;
        }

        public static Image<Gray, byte> OtsuThresholding(Image<Bgr, byte> inputImage)
        {
            if (inputImage == null) return null;

            Image<Gray, byte> grayImage = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte b = inputImage.Data[y, x, 0];
                    byte g = inputImage.Data[y, x, 1];
                    byte r = inputImage.Data[y, x, 2];

                    int gray = (299 * r + 587 * g + 114 * b + 500) / 1000;
                    grayImage.Data[y, x, 0] = (byte)gray;
                }
            }

            return OtsuThresholding(grayImage);
        }
    }
}