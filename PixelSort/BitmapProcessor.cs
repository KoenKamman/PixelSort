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

		// BubbleSort the pixels in the bitmap by luminance
		// TODO: BubbleSort is very inefficient, add other ways to sort
		public WriteableBitmap BubbleSortLuminance(WriteableBitmap bitmap)
		{
			int stride = bitmap.PixelWidth * 4;
			byte[] data = new byte[stride * bitmap.PixelHeight];
			bitmap.CopyPixels(data, stride, 0);

			for (var row = 0; row < bitmap.PixelHeight; row++)
			{
				for (var i = 0; i < bitmap.PixelWidth - 1; i++)
				{
					for (var col = 0; col < bitmap.PixelWidth - 1; col++)
					{
						var index = col * stride + row * 4;

						// Current Pixel
						var currentBlue = data[index];
						var currentGreen = data[index + 1];
						var currentRed = data[index + 2];
						var currentAlpha = data[index + 3];

						// Next Pixel
						var nextBlue = data[index + stride];
						var nextGreen = data[index + stride + 1];
						var nextRed = data[index + stride + 2];
						var nextAlpha = data[index + stride + 3];

						// Calculate luminance
						var currentLuminance = (int)Math.Sqrt(
							currentRed * currentRed * .241 +
							currentGreen * currentGreen * .691 +
							currentBlue * currentBlue * .068);

						var nextLuminance = (int)Math.Sqrt(
							nextRed * nextRed * .241 +
							nextGreen * nextGreen * .691 +
							nextBlue * nextBlue * .068);

						// Swap pixels if luminance of the next pixel is lower
						if (currentLuminance > nextLuminance)
						{
							data[index] = nextBlue;
							data[index + 1] = nextGreen;
							data[index + 2] = nextRed;
							data[index + 3] = nextAlpha;

							data[index + stride] = currentBlue;
							data[index + stride + 1] = currentGreen;
							data[index + stride + 2] = currentRed;
							data[index + stride + 3] = currentAlpha;
						}
					}
				}
			}

			// Specify the rectangle of pixels on the bitmap to be updated
			var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);

			// Write the 1Dimensional Array of pixels to the bitmap
			bitmap.WritePixels(rect, data, stride, 0);

			return bitmap;
		}
	}
}
