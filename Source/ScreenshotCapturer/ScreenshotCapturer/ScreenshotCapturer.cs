using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ScreenshotCapturer
{
	public class ScreenshotCapturer
	{
		public string ImageFilePrefix;
		public string ImageFileSuffix;
		public Rectangle? CaptureBoundsNullable;
		public int ScreenshotsPerSecond;
		public int ScreenshotsToTakeMax;

		public void Start()
		{
			Console.WriteLine("--imageFilePrefix=" + ImageFilePrefix);
			Console.WriteLine("--imageFileSuffix=" + ImageFileSuffix);
			Console.WriteLine("--bounds=" + CaptureBoundsNullable);
			Console.WriteLine("--screenshotsPerSecond=" + ScreenshotsPerSecond);
			Console.WriteLine("--screenshotsMax=" + ScreenshotsToTakeMax);

			if (ScreenshotsToTakeMax == 1)
			{
				CopyScreenToImageFile(0);
			}
			else if (ScreenshotsPerSecond == 0)
			{
				Console.WriteLine("Press the Enter key to take screenshots, or 'quit' to quit.");

				var userWishesToQuit = false;
				var screenshotsTakenSoFarCount = 0;
				while (userWishesToQuit == false)
				{
					var userInput = Console.ReadLine();
					if (userInput == "exit" || userInput == "quit")
					{
						userWishesToQuit = true;
					}
					else
					{
						CopyScreenToImageFile(screenshotsTakenSoFarCount);
						screenshotsTakenSoFarCount++;
						if
						(
							ScreenshotsToTakeMax > 0
							&& screenshotsTakenSoFarCount >= ScreenshotsToTakeMax
						)
						{
							userWishesToQuit = true;
						}
					}
				}
			}
			else
			{
				var millisecondsPerScreenshot =
					(int)Math.Round(1000.0 / ScreenshotsPerSecond);
				for (var i = 0; i < ScreenshotsToTakeMax; i++)
				{
					CopyScreenToImageFile(i);
					Thread.Sleep(millisecondsPerScreenshot);
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
				ImageFilePrefix
				+ (imageIndex == null ? "" : imageIndex.Value.ToString())
				+ ImageFileSuffix;

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