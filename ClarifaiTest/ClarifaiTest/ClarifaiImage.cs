using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClarifaiTest
{
	class ClarifaiImage
	{
		public enum BusyType
		{
			Light,
			Medium,
			Heavy
		};

		public static BusyType StringToBusyType(string type)
		{
			switch (type)
			{
				case "Hilt Light":
					return BusyType.Light;

				case "Hilt Medium":
					return BusyType.Medium;

				case "Hilt Heavy":
					return BusyType.Heavy;

				default:
					throw new ArgumentException();
			}
		}

		public static string BusyTypeToString(BusyType type)
		{
			switch (type)
			{
				case BusyType.Light:
					return "Hilt Light";

				case BusyType.Medium:
					return "Hilt Medium";

				case BusyType.Heavy:
					return "Hilt Heavy";

				default:
					throw new ArgumentException();
			}
		}

		public static BusyType ClarifaiTaggingFromFile(string filename)
		{
			// Convert the stream to a byte array and convert it to base 64 encoding
			Console.WriteLine("Testing image {0}", filename);

			using (var imageStream = new FileStream(filename, FileMode.Open))
			{
				MemoryStream ms = new MemoryStream();
				imageStream.CopyTo(ms);

				return ClarifaiTaggingFromStream(ms);
			}
		}

		public static BusyType ClarifaiTaggingFromStream(MemoryStream image)
		{
			string ACCESS_TOKEN = ConfigurationManager.AppSettings["apiKey"];
			const string CLARIFAI_API_URL = "https://api.clarifai.com/v2/models/Hilt%20Carriage/outputs";

			using (HttpClient client = new HttpClient())
			{
				// Set the authorization header
				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ACCESS_TOKEN);

				string encodedData = Convert.ToBase64String(image.ToArray());

				// The JSON to send in the request that contains the encoded image data
				// Read the docs for more information - https://developer.clarifai.com/guide/predict#predict
				HttpContent json = new StringContent(
					"{" +
						"\"inputs\": [" +
							"{" +
								"\"data\": {" +
									"\"image\": {" +
										"\"base64\": \"" + encodedData + "\"" +
									"}" +
							   "}" +
							"}" +
						"]" +
					"}", Encoding.UTF8, "application/json");

				// Send the request to Clarifai and get a response
				var response = client.PostAsync(CLARIFAI_API_URL, json).Result;

				// Check the status code on the response
				if (!response.IsSuccessStatusCode)
				{
					// End here if there was an error
					Console.WriteLine("Error: {0}, {1}", response.StatusCode, response.ReasonPhrase);
					throw new InvalidDataException();
				}

				// Get response body
				string body = response.Content.ReadAsStringAsync().Result.ToString();

				var jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(body);

				var concepts = jsonResponse.SelectTokens(@"Outputs.Data.Concepts");

				Console.WriteLine("{0}: {1}", (string)jsonResponse.SelectToken("outputs[0].data.concepts[0].name"), (string)jsonResponse.SelectToken("outputs[0].data.concepts[0].value"));
				Console.WriteLine("{0}: {1}", (string)jsonResponse.SelectToken("outputs[0].data.concepts[1].name"), (string)jsonResponse.SelectToken("outputs[0].data.concepts[1].value"));
				Console.WriteLine("{0}: {1}", (string)jsonResponse.SelectToken("outputs[0].data.concepts[2].name"), (string)jsonResponse.SelectToken("outputs[0].data.concepts[2].value"));

				//Console.WriteLine(jsonResponse.ToString());
				return StringToBusyType((string)jsonResponse.SelectToken("outputs[0].data.concepts[0].name"));
			}
		}

		public static void ClarifaiTrainFromStream(MemoryStream image, BusyType type)
		{
			string ACCESS_TOKEN = ConfigurationManager.AppSettings["apiKey"];
			const string CLARIFAI_API_URL = "https://api.clarifai.com/v2/inputs";

			using (HttpClient client = new HttpClient())
			{
				// Set the authorization header
				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ACCESS_TOKEN);

				string encodedData = Convert.ToBase64String(image.ToArray());

				string concept = BusyTypeToString(type);

				// The JSON to send in the request that contains the encoded image data
				// Read the docs for more information - https://developer.clarifai.com/guide/predict#predict
				HttpContent json = new StringContent(
					"{" +
						"\"inputs\": [" +
							"{" +
								"\"data\": {" +
									"\"image\": {" +
										"\"base64\": \"" + encodedData + "\"" +
									"}," +
									"\"concepts\": [" +
									"{ \"id\": \"" + concept + "\", \"value\":\"true\" }" +
									"]" +
							   "}" +
							"}" +
						"]" +
					"}", Encoding.UTF8, "application/json");

				// Send the request to Clarifai and get a response
				var response = client.PostAsync(CLARIFAI_API_URL, json).Result;

				// Check the status code on the response
				if (!response.IsSuccessStatusCode)
				{
					// End here if there was an error
					Console.WriteLine("Error: {0}, {1}", response.StatusCode, response.ReasonPhrase);
				}
				else
				{
					Console.WriteLine("Image added successfully");
				}
			}
		}
	}
}
