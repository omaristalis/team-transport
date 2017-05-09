using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Graphics.Imaging;
using AForge.Video.DirectShow;
using Newtonsoft.Json;

namespace ClarifaiTest
{
	class Program
	{
		static VideoCaptureDevice videoSource = null;
		static System.Drawing.Bitmap lastBitmap = new System.Drawing.Bitmap(1, 1);
		static object bitmapLock = new object();
		static bool shutdown = false;
        static private string BaseUrl = "http://webapplication120170507042553.azurewebsites.net/api/carriages"; //"http://localhost:1331/api/values"; // "http://webapplication120170507042553.azurewebsites.net/api/values";

        private static MemoryStream CaptureCamera()
		{
			if (lastBitmap == null)
				return new MemoryStream();

			MemoryStream imageStream = new MemoryStream();
			lock (bitmapLock)
			{
				lastBitmap.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
			}

			imageStream.Seek(0, SeekOrigin.Begin);

			return imageStream;
		}

		private static void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
		{
			lock (bitmapLock)
			{
				lastBitmap.Dispose();
				lastBitmap = new System.Drawing.Bitmap(eventArgs.Frame);
			}
		}

		static void TagFiles(string path)
		{
			var files = Directory.EnumerateFiles(path);

			foreach ( var file in files )
			{
				Console.WriteLine("Result is {0}", ClarifaiImage.ClarifaiTaggingFromFile(file));
			}

			var directories = Directory.EnumerateDirectories(path);
			
			foreach( var directory in directories)
			{
				TagFiles(directory);
			}
		}

		static public async void TagFilesContinuous()
		{
			Console.WriteLine("Continous Capture Active");

			while (true)
			{
				if (Program.shutdown)
					return;

				Console.WriteLine("Tagging Image");
				var image = CaptureCamera();

				try
				{
					var busyType = ClarifaiImage.ClarifaiTaggingFromStream(image);

				    using (var client = new HttpClient())
				    {
                        var carriageDto = new CarriageDto() { Id = 1, Status = (int)busyType };
                        var content = new StringContent(JsonConvert.SerializeObject(carriageDto), Encoding.UTF8, "application/json");
                        var response = client.PutAsync(BaseUrl + "/" + 1, content).Result;
                    }

				    Console.WriteLine("Image tagged as {0}", busyType.ToString());
				}
				catch (InvalidDataException)
				{
				}

				await Task.Delay(5000);
			}
		}

		static void Main(string[] args)
		{
			//Connect to the camera
			var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

			videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

			videoSource.SnapshotFrame += VideoSource_NewFrame;
			videoSource.NewFrame += VideoSource_NewFrame;

			videoSource.ProvideSnapshots = true;

			videoSource.SnapshotResolution = videoSource.SnapshotCapabilities[0];
			videoSource.Start();

			Task continuousCaptureTask = null;

			while(true)
			{
				Console.WriteLine("1. Add Image");
				Console.WriteLine("2. Tag Image");
				Console.WriteLine("3. Continuously Tag Images");
				Console.WriteLine("0. Exit");

				var option = Console.ReadLine();

				if (option == "0")
				{
					Program.shutdown = true;

					if (continuousCaptureTask != null)
						continuousCaptureTask.Wait();

					videoSource.Stop();
					return;
				}

				switch(option)
				{
					case "1":
						{
							Console.WriteLine("1. Light");
							Console.WriteLine("2. Medium");
							Console.WriteLine("3. Heavy");

							var busy = Console.ReadLine();

							ClarifaiImage.BusyType busyType;
							switch(busy)
							{
								case "1":
									busyType = ClarifaiImage.BusyType.Light;
									break;

								case "2":
									busyType = ClarifaiImage.BusyType.Medium;
									break;

								case "3":
									busyType = ClarifaiImage.BusyType.Heavy;
									break;

								default:
									continue;
							}

							var image = CaptureCamera();

							ClarifaiImage.ClarifaiTrainFromStream(image, busyType);
						}
						break;

					case "2":
						{
							var image = CaptureCamera();

							try
							{
								var busyType = ClarifaiImage.ClarifaiTaggingFromStream(image);
								Console.WriteLine("Image tagged as {0}", busyType.ToString());
							}
							catch (InvalidDataException)
							{
							}
						}
						break;

					case "3":
						continuousCaptureTask = Task.Run(() => TagFilesContinuous());
						break;
				}
			}

			//TagFiles(@"D:\Train carriage photos\photos\");

			//Console.ReadLine();
		}
	}
}
