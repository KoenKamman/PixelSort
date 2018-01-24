using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace PixelSort
{
	public partial class MainWindow
	{
		private readonly BitmapProcessor _bitmapProcessor;
		private WriteableBitmap _bitmap;

		public MainWindow()
		{
			InitializeComponent();
			_bitmapProcessor = new BitmapProcessor();
		}

		private void BtnLoad_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog
			{
				Title = "Open File",
				Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
						 "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
						 "Portable Network Graphic (*.png)|*.png"
			};
			if (ofd.ShowDialog() == true)
			{
				_bitmap = new WriteableBitmap(new BitmapImage(new Uri(ofd.FileName)));
				Image.Source = _bitmap;
			}
		}

		private void BtnSort_Click(object sender, RoutedEventArgs e)
		{
			_bitmap = _bitmapProcessor.BubbleSortLuminance(_bitmap);
			Image.Source = _bitmap;
		}

		private void BtnGenerate_Click(object sender, RoutedEventArgs e)
		{
			_bitmap = _bitmapProcessor.GenerateBitmap(100, 100);
			Image.Source = _bitmap;
		}
	}
}
