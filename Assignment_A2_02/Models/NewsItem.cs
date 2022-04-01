﻿using System;

namespace Assignment_A2_02.Models
{
    public class NewsItem
    {
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public DateTime PublishedAt { get; internal set; }
    }
}
