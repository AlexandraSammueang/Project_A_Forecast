using Assignment_A2_03.Models;
using Assignment_A2_03.ModelsSampleData;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Threading.Tasks;

namespace Assignment_A2_03.Services
{
    public class NewsService
    {
        ConcurrentDictionary<(string, NewsCategory), News> _Cached1 = new ConcurrentDictionary<(string, NewsCategory), News>();
        public EventHandler<string> NewsAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "cc40f1dc262e435b979752a8a9845a75";
        public async Task<News> GetNewsAsync(NewsCategory category)
        {
    
            //NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

            News news = null;

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            NewsCategory cat = category;
            var key = (date,cat);
          
            
            if (!_Cached1.TryGetValue(key, out news))
            {
              
                var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";
                news = await ReadNewsApiAsync(uri);
                _Cached1[key] = news;
                OnNewsAvailable($"News in category availble:{category}");
            }

            else
                OnNewsAvailable($"Cahced News in category availble:{category}");


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

        private NewsItem GetNewsItem(Article newsListItem)
        {
            NewsItem newsitem = new NewsItem();

            newsitem.DateTime = newsListItem.PublishedAt;

            newsitem.Title = newsListItem.Title;

            return newsitem;

        }

    }
}
