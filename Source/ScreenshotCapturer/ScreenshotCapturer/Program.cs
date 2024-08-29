using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScreenshotCapturer
{
	class Program
	{
		static void Main(string[] argsAsArray)
		{
			// Usage:
			// ScreenshotCapturer.exe [--imageFilePrefix=<filePathStemToSaveTo>] [--imageFileSuffix=<imageFileSuffix>]
			// [-bounds=<left> <top> <width> <height>]
			// [-screenshotsPerSecond=<screenshotsPerSecond>] [-screenshotsMax=<screenshotsMax>]
			var now = DateTime.Now;
			Console.WriteLine("Program begins: " + now);

			const string dateTimeFormat = "yyyyMMdd-HHmmss-fff";

			string imageFilePrefix = null;
			string imageFileSuffix = null;
			Rectangle? captureBoundsNullable = null;
			int? screenshotsPerSecond = null;
			int? screenshotsToCaptureMax = null;

			var imageFilePrefixDefault =
				"Screenshot-" + now.ToString(dateTimeFormat);
			const string imageFileSuffixDefault = ".png";
			var captureBoundsDefault = Screen.PrimaryScreen.Bounds;
			const int screenshotsPerSecondDefault = 1;
			const int screenshotsToCaptureMaxDefault = 10;

			try
			{
				var args = argsAsArray.ToList();

				for (var i = 0; i < args.Count; i++)
				{
					var argument = args[i];
					if (argument == "--imageFilePrefix")
					{
						imageFilePrefix = args[i + 1];
						i++;
					}
					else if (argument == "--imageFileSuffix")
					{
						imageFileSuffix = args[i + 1];
						i++;
					}
					else if (argument == "--bounds")
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
					else if (argument == "--screenshotsPerSecond")
					{
						screenshotsPerSecond = int.Parse(args[i + 1]);
						i++;
					}
					else if (argument == "--screenshotsMax")
					{
						screenshotsToCaptureMax = int.Parse(args[i + 1]);
						i++;
					}
					else
					{
						throw new Exception("Unrecognized command-line switch:" + argument);
					}
				}

				var isInteractive =
					args.Count == 0
					|| args.Contains("--interactive");

				if (isInteractive)
				{
					if (imageFilePrefix == null)
					{
						Console.Write($"Image File Prefix [{imageFilePrefixDefault}]:");
						imageFilePrefix = Console.ReadLine();
						if (imageFilePrefix == "")
						{
							imageFilePrefix = imageFilePrefixDefault;
						}
					}

					if (imageFileSuffix == null)
					{
						Console.Write($"Image File Suffix [{imageFileSuffixDefault}]:");
						imageFileSuffix = Console.ReadLine();
						if (imageFileSuffix == "")
						{
							imageFileSuffix = imageFileSuffixDefault;
						}
					}
					
					if (captureBoundsNullable == null)
					{
						var captureBoundsDefaultAsString =
							captureBoundsDefault.X
							+ " " + captureBoundsDefault.Y
							+ " " + captureBoundsDefault.Width
							+ " " + captureBoundsDefault.Height;

						int[] captureBoundsXYWidthAndHeight = null;

						Console.Write($"Bounds to capture image within in pixels (left top width height) [{captureBoundsDefaultAsString }]:");
						var captureBoundsAsString = Console.ReadLine();
						if (captureBoundsAsString == "")
						{
							captureBoundsXYWidthAndHeight = new int[]
							{
								captureBoundsDefault.X, captureBoundsDefault.Y,
								captureBoundsDefault.Width, captureBoundsDefault.Height
							};
						}
						else
						{
							try
							{
								captureBoundsXYWidthAndHeight =
									captureBoundsAsString
										.Split(" ".ToCharArray())
										.Select(x => int.Parse(x))
										.ToArray();
							}
							catch (FormatException)
							{
								Console.WriteLine("Bounds did not have the expected format.  Using default.");
								captureBoundsXYWidthAndHeight = new int[]
								{
									captureBoundsDefault.X, captureBoundsDefault.Y,
									captureBoundsDefault.Width, captureBoundsDefault.Height
								};
							}
						}

						if (captureBoundsXYWidthAndHeight.Length != 4)
						{
							Console.WriteLine("Bounds did not have the expected format.  Using default.");
							captureBoundsNullable = captureBoundsDefault;
						}
						else
						{
							captureBoundsNullable = new Rectangle
							(
								captureBoundsXYWidthAndHeight[0],
								captureBoundsXYWidthAndHeight[1],
								captureBoundsXYWidthAndHeight[2],
								captureBoundsXYWidthAndHeight[3]
							);
						}
					}

					if (screenshotsPerSecond == null)
					{
						Console.Write($"Screenshots per second: (0 for user-triggered) [{screenshotsPerSecondDefault}]:");
						var screenshotsPerSecondAsString = Console.ReadLine();
						if (screenshotsPerSecondAsString == "")
						{
							screenshotsPerSecond = screenshotsPerSecondDefault;
						}
						else
						{
							try
							{
								screenshotsPerSecond = int.Parse(screenshotsPerSecondAsString);
							}
							catch (FormatException)
							{
								Console.WriteLine("Screenshots per second did not have expected format.  Using default.");
								screenshotsPerSecond = screenshotsPerSecondDefault;
							}
						}
					}

					if (screenshotsToCaptureMax == null)
					{
						Console.Write($"Maximum number of screenshots to take (0 for infinity) [{screenshotsToCaptureMaxDefault}]:");
						var numberOfScreenshotsToCaptureAsString = Console.ReadLine();
						if (numberOfScreenshotsToCaptureAsString == "")
						{
							screenshotsToCaptureMax = screenshotsToCaptureMaxDefault;
						}
						else
						{
							try
							{
								screenshotsToCaptureMax = int.Parse(numberOfScreenshotsToCaptureAsString);
								if (screenshotsToCaptureMax < 0)
								{
									Console.WriteLine("Number of screenshots to take did not have the expected format.  Using default.");
								}
							}
							catch (FormatException)
							{
								Console.WriteLine("Number of screenshots to take did not have the expected format.  Using default.");
								screenshotsToCaptureMax = screenshotsToCaptureMaxDefault;
							}
						}
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
				ImageFilePrefix = imageFilePrefix,
				ImageFileSuffix = imageFileSuffix,
				CaptureBoundsNullable = captureBoundsNullable,
				ScreenshotsPerSecond = screenshotsPerSecond.Value,
				ScreenshotsToTakeMax = screenshotsToCaptureMax.Value,
			};

			screenshotCapturer.Start();

			Console.WriteLine("Program ends: " + DateTime.Now);

			if (Debugger.IsAttached)
			{
				Console.Write("Press the Enter key to quit.");
				Console.ReadLine();
			}
		}
	}
}