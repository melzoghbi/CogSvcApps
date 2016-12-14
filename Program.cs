using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ComputerVisionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = string.Empty;
            string key = string.Empty;


            if (args != null && args.Count()>0)
            {
                url = args[0];
            }
            else
            {
                Console.WriteLine("Please paste an image url to analyze!");
                url = Console.ReadLine();
            }

            key = ConfigurationManager.AppSettings["computerVisionKey"];

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(key))
            {
                MakeRequest(url, key);
                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Please provide an image url and a computer vision api key");
            }
        }
        static async void MakeRequest(string url, string key)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            // Request parameters
            queryString["maxCandidates"] = "1";
            var uri = "https://api.projectoxford.ai/vision/v1.0/describe?" + queryString;

            HttpResponseMessage response;
          
            // Request body
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { url = url });

            byte[] byteData = Encoding.UTF8.GetBytes(json);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                   JObject resp = JObject.Parse( await response.Content.ReadAsStringAsync());
                  
                    Console.WriteLine("\nThe image tags are:");
                    foreach (var item in resp["description"]["tags"])
                    {
                        Console.Write(item + "\t");
                    }

                    Console.WriteLine("\n\nThe image captions are:");
                    foreach (var item in resp["description"]["captions"])
                    {
                        Console.WriteLine(item["text"] + "\t" + "with confidence: " + item["confidence"]);
                    }

                    Console.WriteLine("\nThe image metadata below:");
                    Console.WriteLine("width: " + resp["metadata"]["width"]);
                    Console.WriteLine("height: " + resp["metadata"]["height"]);
                    Console.WriteLine("format: " + resp["metadata"]["format"]);

                }
            }

        }
    }
}
