using Assignment_A2_02.Models;
using Assignment_A2_02.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_A2_02
{
    class Program
    {
        static void Main(string[] args)
        {
            NewsService service = new NewsService();
            Console.WriteLine("---------------------------");
            for (NewsCategory i = NewsCategory.business; i < NewsCategory.technology + 1; i++)
            {
                Task<News> t1 = service.GetNewsAsync(i);
                Task.WaitAll(t1);

                if (t1?.Status == TaskStatus.RanToCompletion)
                {
                    News news2 = t1.Result;
                    Console.WriteLine($"News in Category {i}");

                    news2.Articles.ForEach(a => Console.WriteLine($" - {a.DateTime.ToString("yyyy-MM-dd HH:mm-ss")}\t: {a.Title}"));
                   
                }
                else
                {
                    Console.WriteLine($"Geolocation News service error.");
                }

            }




        }
    }
}
