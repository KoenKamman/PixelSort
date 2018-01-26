using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
		public async Task<WriteableBitmap> BubbleSortLuminance(WriteableBitmap bitmap, IProgress<float> progress)
		{
			return await Task.Run(() =>
			{
				int backBufferStride = 0, width = 0, height = 0, bytesPerPixel = 0;
				IntPtr pBackBuffer = IntPtr.Zero;

				// UI Thread pre-update
				Application.Current.Dispatcher.Invoke(() =>
				{
					// Lock the bitmap and get a pointer to the backbuffer
					bitmap.Lock();
					pBackBuffer = bitmap.BackBuffer;
					backBufferStride = bitmap.BackBufferStride;
					width = bitmap.PixelWidth;
					height = bitmap.PixelHeight;
					bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
				});

				unsafe
				{
					byte* pImgData = (byte*)pBackBuffer;

					int cRowStart = 0;
					for (int row = 0; row < height; row++)
					{
						for (int i = 0; i < width - 1; i++)
						{
							var cColStart = cRowStart;
							for (int col = 0; col < width - 1; col++)
							{
								byte* bPixel = pImgData + cColStart;
								byte* bPixelNext = pImgData + cColStart + bytesPerPixel;

								// Current Pixel
								var currentBlue = bPixel[0];
								var currentGreen = bPixel[1];
								var currentRed = bPixel[2];
								var currentAlpha = bPixel[3];

								// Next Pixel
								var nextBlue = bPixelNext[0];
								var nextGreen = bPixelNext[1];
								var nextRed = bPixelNext[2];
								var nextAlpha = bPixelNext[3];

								// Calculate luminance
								var currentLuminance = CalcLuminance(currentRed, currentGreen, currentBlue);
								var nextLuminance = CalcLuminance(nextRed, nextGreen, nextBlue);

								// Swap pixels if luminance of the next pixel is lower
								if (currentLuminance > nextLuminance)
								{
									bPixel[0] = nextBlue;
									bPixel[1] = nextGreen;
									bPixel[2] = nextRed;
									bPixel[3] = nextAlpha;

									bPixelNext[0] = currentBlue;
									bPixelNext[1] = currentGreen;
									bPixelNext[2] = currentRed;
									bPixelNext[3] = currentAlpha;
								}

								cColStart += bytesPerPixel;
							}
						}
						cRowStart += backBufferStride;
						progress?.Report((float)100 / height * row + 1);
					}
				}

				// UI Thread post-update
				Application.Current.Dispatcher.Invoke(() =>
				{
					// Specify which pixels changed and unlock the bitmap
					bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
					bitmap.Unlock();
				});

				return bitmap;
			});
		}

		private static int CalcLuminance(int red, int green, int blue)
		{
			return (int)Math.Sqrt(
				red * red * .241 +
				green * green * .691 +
				blue * blue * .068);
		}
	}
}
