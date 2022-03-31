using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {
        ConcurrentDictionary<(string, string), Forecast> _Cached1 = new ConcurrentDictionary<(string, string), Forecast>();
        ConcurrentDictionary<(string,double, double), Forecast> _Cached2 = new ConcurrentDictionary<(string,double, double), Forecast>();
        public EventHandler<string> WeatherForecastAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "c804308f61ca726506e158cad6676dd6"; // Your API Key


        // part of your event and cache code here

       public async Task<Forecast> GetForecastAsync(string City)
        {
            //part of cache code here

           Forecast forecast = null;
            
             string date= DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                string city = City;
                var key = (date, city);
                if (!_Cached1.TryGetValue(key, out forecast))
                {
                    //https://openweathermap.org/current
                    var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

                    forecast = await ReadWebApiAsync(uri);
                    _Cached1[key] = forecast;
                    OnWeatherForecastAvailable($"New weather forecast for {City} available");
                }

                else
                    OnWeatherForecastAvailable($"Caheced weather forecast for {City} available");


                //part of event and cache code here
                //generate an event with different message if cached data               
            

            return forecast;

        }
        protected virtual void OnWeatherForecastAvailable(string s)
        {
            WeatherForecastAvailable?.Invoke(this, s);
        }


        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //part of cache code here

            Forecast forecast = null;

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            double lati = latitude;
            double longi = longitude;
            var key = (date,lati, longi);

           
            if (!_Cached2.TryGetValue(key, out forecast))
            {
                //https://openweathermap.org/current
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

                 forecast = await ReadWebApiAsync(uri);
                _Cached2[key] = forecast;

                //part of event and cache code here
                //generate an event with different message if cached data

                OnWeatherForecastAvailable($"New weather forecast for ({latitude}, {longitude}) available");
            }
            else
                OnWeatherForecastAvailable($"Cached weather forecast for ({latitude}, {longitude}) available");



            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            // part of your read web api code here

            // part of your data transformation to Forecast here
            //generate an event with different message if cached data

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();
            Forecast forecast = new Forecast();

            forecast.City = wd.city.name;


            forecast.Items = new List<ForecastItem>();

            wd.list.ForEach(wdListItem => { forecast.Items.Add(GetForecastItem(wdListItem)); });
            return forecast;

           
        }

        private ForecastItem GetForecastItem(List wdListItem)
        {

            ForecastItem item = new ForecastItem();
            item.DateTime = UnixTimeStampToDateTime(wdListItem.dt);

            item.Temperature = wdListItem.main.temp;
            item.Description = wdListItem.weather.Count > 0 ? wdListItem.weather.First().description : "No info";
            item.WindSpeed = wdListItem.wind.speed;

            return item;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
