using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PixelSort
{
	internal class Sort
	{
		public WriteableBitmap BubbleSort(WriteableBitmap bitmap, Image image)
		{
			for (var row = 0; row < bitmap.PixelWidth; row++)
			{
				for (var i = 0; i < bitmap.PixelWidth - 1; i++)
				{
					for (var col = 0; col < bitmap.PixelWidth - 1; col++)
					{
						var pixel = bitmap.GetPixel(row, col);
						var nextPixel = bitmap.GetPixel(row, col + 1);
						if (pixel.R > nextPixel.R)
						{
							bitmap.SetPixel(row, col, nextPixel.A, nextPixel.R, nextPixel.G, nextPixel.B);
							bitmap.SetPixel(row, col + 1, pixel.A, pixel.R, pixel.G, pixel.B);
							image.Source = bitmap;
						}
					}
				}
			}
			return bitmap;
		}
	}
}
