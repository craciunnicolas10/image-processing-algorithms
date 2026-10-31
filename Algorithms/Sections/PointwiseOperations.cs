using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        public static Image<Gray, byte> ApplyLUT(Image<Gray, byte> inputImage, byte[]lut)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for(int y=0;y<inputImage.Height;y++) {
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
        #region BrightnessContrast
        public static byte[] ComputeBrightnessContrastLUT(double a, double b)
        {
            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double value = a * i + b + 0.5;
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                lut[i] = (byte)value;
            }
            return lut;
        }

        public static Image<Gray, byte> BrightnessContrast(Image<Gray, byte> inputImage, double a, double b)
        {
            byte[] lut = ComputeBrightnessContrastLUT(a, b);
            return ApplyLUT(inputImage, lut);
        }

        public static Image<Bgr, byte> BrightnessContrast(Image<Bgr, byte> inputImage, double a, double b)
        {
            byte[] lut = ComputeBrightnessContrastLUT(a, b);
            return ApplyLUTColor(inputImage, lut);
        }
        #endregion

        #region Gamma
        public static byte[] ComputeGammaLUT(double gamma)
        {
            byte[] lut = new byte[256];
            double c = 255.0 / System.Math.Pow(255.0, gamma);
            for (int i = 0; i < 256; i++)
            {
                double value = c * System.Math.Pow(i, gamma) + 0.5;
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                lut[i] = (byte)value;
            }
            return lut;
        }

        public static Image<Gray, byte> GammaOperator(Image<Gray, byte> inputImage, double gamma)
        {
            byte[] lut = ComputeGammaLUT(gamma);
            return ApplyLUT(inputImage, lut);
        }

        public static Image<Bgr, byte> GammaOperator(Image<Bgr, byte> inputImage, double gamma)
        {
            byte[] lut = ComputeGammaLUT(gamma);
            return ApplyLUTColor(inputImage, lut);
        }
        #endregion
    }
}