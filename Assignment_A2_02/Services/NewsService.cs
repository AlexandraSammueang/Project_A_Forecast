#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using Assignment_A2_02.Models;
using Assignment_A2_02.ModelsSampleData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Threading.Tasks;

namespace Assignment_A2_02.Services
{
    public class NewsService
    {
        public EventHandler<string> NewsAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "cc40f1dc262e435b979752a8a9845a75";
        public async Task<News> GetNewsAsync(NewsCategory category)
        {
#if UseNewsApiSample      
            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

#else
            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";

           // Your code here to get live data

#endif

            //var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";

            //HttpResponseMessage response = await httpClient.GetAsync(uri);
            //response.EnsureSuccessStatusCode();

            //NewsApiData nd1 = await response.Content.ReadFromJsonAsync<NewsApiData>();

            //Your Code to convert WeatherApiData to Forecast using Linq.
            News news = new News();

            news.Articles = new List<NewsItem>();

            nd.Articles.ForEach(a => { news.Articles.Add(GetNewsItem(a));});

            OnNewsAvailable($"News in category availble:{category}");


            return news;

        }
        protected virtual void OnNewsAvailable(string c)
        {
            NewsAvailable?.Invoke(this, c);
        }

        private NewsItem GetNewsItem(Article wdListItem)
        {
            NewsItem newsitem = new NewsItem();

            newsitem.DateTime = wdListItem.PublishedAt;

            newsitem.Title = wdListItem.Title;

            return newsitem;

        }

    }
}
