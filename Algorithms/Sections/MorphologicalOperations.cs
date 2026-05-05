using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        public static Image<Bgr, byte> ConnectedComponents(Image<Gray, byte> inputImage)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;

            Image<Gray, byte> binaryImage = Thresholding.OtsuThresholding(inputImage);

            Image<Bgr, byte> result = new Image<Bgr, byte>(width, height);
            bool[,] visited = new bool[height, width];
            Random rand = new Random();

            int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (binaryImage.Data[y, x, 0] == 255 && !visited[y, x])
                    {
                        byte b = (byte)rand.Next(0, 256);
                        byte g = (byte)rand.Next(0, 256);
                        byte r = (byte)rand.Next(0, 256);

                        Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
                        queue.Enqueue(new Tuple<int, int>(x, y));
                        visited[y, x] = true;

                        while (queue.Count > 0)
                        {
                            var current = queue.Dequeue();
                            int cx = current.Item1;
                            int cy = current.Item2;

                            result.Data[cy, cx, 0] = b;
                            result.Data[cy, cx, 1] = g;
                            result.Data[cy, cx, 2] = r;

                            for (int i = 0; i < 8; i++)
                            {
                                int nx = cx + dx[i];
                                int ny = cy + dy[i];

                                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                                {
                                    if (binaryImage.Data[ny, nx, 0] == 255 && !visited[ny, nx])
                                    {
                                        visited[ny, nx] = true;
                                        queue.Enqueue(new Tuple<int, int>(nx, ny));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}