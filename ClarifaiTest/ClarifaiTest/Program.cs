﻿using System;
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

namespace ClarifaiTest
{
	class Program
	{
		static VideoCaptureDevice videoSource = null;
		static System.Drawing.Bitmap lastBitmap = new System.Drawing.Bitmap(1, 1);

		private static MemoryStream CaptureCamera()
		{
			if (lastBitmap == null)
				return new MemoryStream();

			MemoryStream imageStream = new MemoryStream();
			lock (lastBitmap)
			{
				lastBitmap.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
			}

			imageStream.Seek(0, SeekOrigin.Begin);

			return imageStream;
		}

		private static void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
		{
			lock (lastBitmap)
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

			while(true)
			{
				Console.WriteLine("1. Add Image");
				Console.WriteLine("2. Tag Image");
				Console.WriteLine("0. Exit");

				var option = Console.ReadLine();

				if (option == "0")
					return;

				var image = CaptureCamera();

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

							ClarifaiImage.ClarifaiTrainFromStream(image, busyType);
						}
						break;

					case "2":
						{
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
				}
			}

			//TagFiles(@"D:\Train carriage photos\photos\");

			//Console.ReadLine();
		}
	}
}