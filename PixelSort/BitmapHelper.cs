using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixelSort
{
	internal class BitmapHelper
	{
		private readonly int _width, _height;
		private readonly byte[,,] _pixels;
		private readonly int _stride;

		public BitmapHelper(int width, int height)
		{
			_width = width;
			_height = height;
			_pixels = new byte[height, width, 4];
			_stride = width * 4;
		}

		public WriteableBitmap GenerateBitmap()
		{

			var wbitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgra32, null);

			// Fill with color
			var rnd = new Random();
			for (var row = 0; row < _height; row++)
			{
				for (var col = 0; col < _width; col++)
				{
					//// Blue, Green, Red
					//for (var i = 0; i < 3; i++)
					//{
					//	_pixels[row, col, i] = (byte)rnd.Next(0, 255);
					//}

					//Blue
					_pixels[row, col, 0] = 0;
					//Green
					_pixels[row, col, 1] = 0;
					//Red
					_pixels[row, col, 2] = (byte)rnd.Next(50, 255);
					//Alpha
				   _pixels[row, col, 3] = 255;
				}
			}

			// Copy the data into a one-dimensional array
			var pixels1D = new byte[_height * _width * 4];
			var index = 0;
			for (var row = 0; row < _height; row++)
			{
				for (var col = 0; col < _width; col++)
				{
					for (var i = 0; i < 4; i++)
						pixels1D[index++] = _pixels[row, col, i];
				}
			}

			// Update WriteableBitmap
			var rect = new Int32Rect(0, 0, _width, _height);
			wbitmap.WritePixels(rect, pixels1D, _stride, 0);

			return wbitmap;
		}
	}
}
