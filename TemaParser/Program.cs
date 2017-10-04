using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace TemaParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string uriToDownload = @"https://www.artlebedev.ru/kovodstvo/business-lynch/tema/";
            getContent(uriToDownload);
        }

        static HtmlDocument getContent(string uri)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(uri);
        }
    }
}
