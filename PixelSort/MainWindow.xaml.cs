using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace PixelSort
{
	public partial class MainWindow : Window
	{
		private WriteableBitmap _bitmap;
		private Sort _sorter = new Sort();
		private BitmapHelper helper = new BitmapHelper(50, 50);

		public MainWindow()
		{
			InitializeComponent();
		}

		private void BtnGenerate_Click(object sender, RoutedEventArgs e)
		{
			//var ofd = new OpenFileDialog
			//{
			//	Title = "Select a picture",
			//	Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
			//	         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
			//	         "Portable Network Graphic (*.png)|*.png"
			//};
			//if (ofd.ShowDialog() == true)
			//{
			//	_bitmap = new WriteableBitmap(new BitmapImage(new Uri(ofd.FileName)));
			//}
			_bitmap = helper.GenerateBitmap();
			Image.Source = _bitmap;
		}

		private void BtnSort_Click(object sender, RoutedEventArgs e)
		{
			// var newBitmap = _sorter.BubbleSort(_bitmap, Image);
			var newBitmap = _sorter.BubbleSort(_bitmap, Image);
			Image.Source = newBitmap;
		}
	}
}
