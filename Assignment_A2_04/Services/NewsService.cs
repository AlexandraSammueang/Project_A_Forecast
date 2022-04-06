#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using Assignment_A2_04.Models;
using Assignment_A2_04.ModelsSampleData;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Threading.Tasks;

namespace Assignment_A2_04.Services
{
    public class NewsService
    {
      
        public EventHandler<string> NewsAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "cc40f1dc262e435b979752a8a9845a75";

        public async Task<News> GetNewsAsync(NewsCategory category)
        {
#if UseNewsApiSample      
            //NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

#else
            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";

           // Your code here to get live data

#endif   
            var dt = DateTime.Now;
            
            NewsCacheKey key = new(category, dt);
            News news = null;

            if (!key.CacheExist)
            {

                var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";
                news = await ReadNewsApiAsync(uri);
                News.Serialize(news, key.FileName);
                OnNewsAvailable($"News in category availble:{category}");
            }
            else
            {
                News.Deserialize(key.FileName);
                OnNewsAvailable($"XML Cached in category availble:{category}");
            }

            return news;
            

        }
        protected virtual void OnNewsAvailable(string c)
        {
            NewsAvailable?.Invoke(this, c);
        }

        private async Task<News> ReadNewsApiAsync(string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();

            News news = new News();

            news.Articles = new List<NewsItem>();

            nd.Articles.ForEach(a => { news.Articles.Add(GetNewsItem(a)); });

            return news;

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
