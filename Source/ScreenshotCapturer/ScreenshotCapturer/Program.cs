using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ScreenshotCapturer
{
	class Program
	{
		static void Main(string[] args)
		{
			// Usage:
			// ScreenshotCapturer.exe [-prefix <filePathStemToSaveTo>] [-extension <imageFileExtension>]
			// [-bounds <captureBoundsX> <captureBoundsY> <captureBoundsWidth> <captureBoundsHeight>]
			// [-count <numberOfScreenshotsToCapture>] [-frequency <screenshotsPerSecond>]
			var now = DateTime.Now;
			Console.WriteLine("Program begins: " + now);
			const string dateTimeFormat = "yyyyMMdd-HHmmss-fff";
			var filePathStemToSaveTo = "Screenshot-" + now.ToString(dateTimeFormat);
			var imageFileExtension = ".png";
			Rectangle? captureBoundsNullable = null;
			var numberOfScreenshotsToCapture = 1;
			var millisecondsPerScreenshot = 1000;
			try
			{
				for (var i = 0; i < args.Length; i++)
				{
					var argument = args[i];
					if (argument == "-prefix")
					{
						filePathStemToSaveTo = args[i + 1];
						i++;
					}
					else if (argument == "-extension")
					{
						imageFileExtension = args[i + 1];
						i++;
					}
					else if (argument == "-bounds")
					{
						captureBoundsNullable = new Rectangle
						(
							int.Parse(args[i + 1]),
							int.Parse(args[i + 2]),
							int.Parse(args[i + 3]),
							int.Parse(args[i + 4])
						);
						i += 4;
					}
					else if (argument == "-count")
					{
						numberOfScreenshotsToCapture = int.Parse(args[i + 1]);
						i++;
					}
					else if (argument == "-frequency")
					{
						var screenshotsPerSecond = int.Parse(args[i + 1]);
						millisecondsPerScreenshot = 1000 / screenshotsPerSecond; i++;
					}
					else
					{
						throw new Exception("Unrecognized command-line switch:" + argument);
					}
				}
			}
			catch (Exception ex)
			{
				var errorMessage =
					"Error parsing command-line arguments: " + ex.Message;
				Console.WriteLine(errorMessage);
			}
			var screenshotCapturer = new ScreenshotCapturer()
			{
				FilePathStemToSaveTo = filePathStemToSaveTo,
				ImageFileExtension = imageFileExtension,
				CaptureBoundsNullable = captureBoundsNullable,
				NumberOfScreenshotsToCapture = numberOfScreenshotsToCapture,
				MillisecondsPerScreenshot = millisecondsPerScreenshot
			};
			screenshotCapturer.Start();
			Console.WriteLine("Program ends: " + DateTime.Now);
		}
	}
	public class ScreenshotCapturer
	{
		public string FilePathStemToSaveTo;
		public string ImageFileExtension;
		public Rectangle? CaptureBoundsNullable;
		public int NumberOfScreenshotsToCapture;
		public int MillisecondsPerScreenshot;

		public void Start()
		{
			Console.WriteLine("-prefix=" + FilePathStemToSaveTo);
			Console.WriteLine("-extension=" + ImageFileExtension);
			Console.WriteLine("-bounds=" + CaptureBoundsNullable);
			Console.WriteLine("-count=" + NumberOfScreenshotsToCapture);
			Console.WriteLine("-frequency=" + (1000 / MillisecondsPerScreenshot));
			if (NumberOfScreenshotsToCapture == 1)
			{
				CopyScreenToImageFile(0);
			}
			else
			{
				for (var i = 0; i < NumberOfScreenshotsToCapture; i++)
				{
					CopyScreenToImageFile(i);
					Thread.Sleep(MillisecondsPerScreenshot);
				}
			}
		}
		public void CopyScreenToImageFile(int? imageIndex)
		{
			// Adapted from code found at the URL
			// https://stackoverflow.com/questions/5049122/capture-the-screen-shot-using-net
			Rectangle captureBounds;
			if (CaptureBoundsNullable == null)
			{
				captureBounds = Screen.PrimaryScreen.Bounds;
			}
			else
			{
				captureBounds = CaptureBoundsNullable.Value;
			}
			var bitmapCaptured = new Bitmap(captureBounds.Width, captureBounds.Height);
			var graphics = Graphics.FromImage(bitmapCaptured);
			graphics.CopyFromScreen
			(
				captureBounds.X, captureBounds.Y, // source
				0, 0, // destination
				bitmapCaptured.Size,
				CopyPixelOperation.SourceCopy
			);
			var filePathToSaveTo =
				FilePathStemToSaveTo
				+ (imageIndex == null ? "" : imageIndex.Value.ToString())
				+ ImageFileExtension;

			Console.WriteLine("Saving " + filePathToSaveTo + "...");

			try
			{
				bitmapCaptured.Save(filePathToSaveTo);
			}
			catch (Exception)
			{
				var errorMessage =
					"Error attemping to save file. Ensure directory exists, and permissions are adequate.";
				Console.WriteLine(errorMessage);
			}
		}
	}
}