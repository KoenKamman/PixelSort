using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixelSort
{
	internal class BitmapProcessor
	{
		public WriteableBitmap GenerateBitmap(int width, int height)
		{
			// Create a new WriteableBitmap
			// TODO: Look at BitmapPalette
			WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

			// Create a new 3Dimensional Array with 4 bytes for each pixel (1 byte for each color & alpha)
			byte[,,] pixels3D = new byte[height, width, 4];

			// Width of a single row of pixels rounded up to a four-byte boundary
			int stride = width * 4;

			// Fill pixels3D with random colors
			Random rnd = new Random();
			for (int row = 0; row < height; row++)
			{
				for (int col = 0; col < width; col++)
				{
					//Blue
					pixels3D[row, col, 0] = (byte)rnd.Next(0, 255);
					//Green
					pixels3D[row, col, 1] = (byte)rnd.Next(0, 255);
					//Red
					pixels3D[row, col, 2] = (byte)rnd.Next(0, 255);
					//Alpha
					pixels3D[row, col, 3] = 255;
				}
			}

			// Copy the 3Dimensional Array to a 1Dimensional Array
			// (Height * Width * 4) calculates the amount of bytes needed to store all color data
			var pixels1D = new byte[height * width * 4];
			var index = 0;
			for (var row = 0; row < height; row++)
			{
				for (var col = 0; col < width; col++)
				{
					for (var i = 0; i < 4; i++)
					{
						pixels1D[index++] = pixels3D[row, col, i];
					}
				}
			}

			// Specify the rectangle of pixels on the bitmap to be updated
			var rect = new Int32Rect(0, 0, width, height);

			// Write the 1Dimensional Array of pixels to the bitmap
			bitmap.WritePixels(rect, pixels1D, stride, 0);

			return bitmap;
		}

		public WriteableBitmap BubbleSortLuminance(WriteableBitmap bitmap)
		{
			for (var row = 0; row < bitmap.PixelWidth; row++)
			{
				for (var i = 0; i < bitmap.PixelHeight - 1; i++)
				{
					for (var col = 0; col < bitmap.PixelHeight - 1; col++)
					{
						// Get pixels
						var pixel = bitmap.GetPixel(row, col);
						var nextPixel = bitmap.GetPixel(row, col + 1);

						// Calculate luminance
						var pixelLuminance = (0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B);
						var nextPixelLuminance = (0.2126 * nextPixel.R + 0.7152 * nextPixel.G + 0.0722 * nextPixel.B);

						// Swap pixels if luminance of the next pixel is lower
						if (pixelLuminance > nextPixelLuminance)
						{
							bitmap.SetPixel(row, col, nextPixel.A, nextPixel.R, nextPixel.G, nextPixel.B);
							bitmap.SetPixel(row, col + 1, pixel.A, pixel.R, pixel.G, pixel.B);
						}
					}
				}
			}
			return bitmap;
		}
	}
}
