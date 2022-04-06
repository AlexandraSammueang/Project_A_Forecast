using Assignment_A2_01.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Threading.Tasks;
namespace Assignment_A2_01.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "cc40f1dc262e435b979752a8a9845a75";

        public async Task<NewsApiData> GetNewsAsync()
        {
            //NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync("sports");

            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category=sports&apiKey={apiKey}";

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();

            NewsApiData newsapidata = new NewsApiData();

            newsapidata.Articles = new List<Article>();

            nd.Articles.ForEach(a => { newsapidata.Articles.Add(a);});

            return newsapidata;
        }

    }
}