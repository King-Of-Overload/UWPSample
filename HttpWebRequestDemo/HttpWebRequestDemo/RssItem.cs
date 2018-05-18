﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpWebRequestDemo
{
    public class RssItem
    {
        public string Title { get; set; }//标题

        public string Summary { get; set; }//内容

        public string PublishedDate { get; set; }//发表时间

        public string Url { get; set; }//文章地址

        public string PlainSummary { get; set; }//解析的文本内容

        public RssItem(string title, string summary, string publishedDate, string url)
        {
            Title = title;
            Summary = summary;
            PublishedDate = publishedDate;
            Url = url;
            //解析html
            PlainSummary = WebUtility.HtmlDecode(Regex.Replace(summary, "<[^>]+?>", ""));
        }
    }
}
