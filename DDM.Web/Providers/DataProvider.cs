using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace DDM.Web.Providers
{
    public class DataProvider : IDataProvider
    {
        private readonly Uri baseUri;
        private readonly HttpClient client;

        public DataProvider(string baseUrl)
        {
            baseUri = new Uri(baseUrl);
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(@"application/json"));
        }

        public T GetData<T>(string url)
        {
            HttpResponseMessage response = client.GetAsync(new Uri(baseUri, url)).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }

            throw new HttpRequestException(response.StatusCode.ToString());
        }
    }
}