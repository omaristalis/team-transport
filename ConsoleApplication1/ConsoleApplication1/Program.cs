using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Newtonsoft.Json;

    class Program
    {
        static private string BaseUrl = "http://webapplication120170507042553.azurewebsites.net/api/carriages"; //"http://localhost:1331/api/values"; // "http://webapplication120170507042553.azurewebsites.net/api/values";

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Arguments: Offset Value");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var carriageDto = new CarriageDto() {Id = int.Parse(args[0]), Status = int.Parse(args[1]) };
                    var content = new StringContent(JsonConvert.SerializeObject(carriageDto), Encoding.UTF8, "application/json");

                    var response = client.PutAsync(BaseUrl + "/" + args[0], content).Result;
                    Console.WriteLine("Put: " + response.StatusCode);

                    response = client.GetAsync(BaseUrl + "/" + args[0]).Result;
                    var text = response.Content.ReadAsStringAsync().Result;
                    CarriageDto dto = JsonConvert.DeserializeObject<CarriageDto>(text);
                    Console.WriteLine("Get: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
