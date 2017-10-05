using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using Newtonsoft.Json;

namespace TemaParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string uriToDownload = @"https://www.artlebedev.ru/kovodstvo/business-lynch/tema/";
            var reviewsList = new List<Review>();
            var reviewsPage = getContent(uriToDownload);
            HtmlNodeCollection reviewLinks = reviewsPage.DocumentNode.SelectNodes("//td/div/span | //td/div/a");
            //for (int i = 0; i < reviewLinks.Count; i += 2)
            for (int i = 0; i < 20; i += 2)
            {
                try
                {
                    reviewsList.Add(new Review { date = reviewLinks[i].InnerHtml.ToString() });
                    reviewsList[reviewsList.Count - 1].link = "https://www.artlebedev.ru" + reviewLinks[i + 1].OuterHtml.ToString().Split('"')[1];
                    var reviewPage = getContent(reviewsList[reviewsList.Count - 1].link);
                    reviewsList[reviewsList.Count - 1].comments = getReviewComments(reviewPage);
                    reviewsList[reviewsList.Count - 1].text = getReviewText(reviewPage);
                    Console.WriteLine("Parsed " + (i / 2 + 1) + " pages!");
                }
                catch
                {
                    reviewsList.Remove(reviewsList[reviewsList.Count - 1]); //remove invalid element from list
                }
            }
            Review.saveToJson(reviewsList);
            Console.WriteLine("Json is ready!");
            Console.ReadKey();
        }

        //get html page using uri
        static HtmlDocument getContent(string uri)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(uri);
        }

        //get Tema's comments
        static List<string> getReviewComments(HtmlDocument hd)
        {
            var list = new List<string>();
            HtmlNodeCollection comments = hd.DocumentNode.SelectNodes("//div[contains(@class, 'LynchComment')]");
            foreach(HtmlNode hn in comments)
            {
                list.Add(hn.InnerText);
            }
            return list;
        }

        //get Title text
        static string getReviewText(HtmlDocument hd)
        {
            return hd.DocumentNode.SelectNodes("//div[contains(@class, 'value')]")[1].InnerText;
        }
    }

    class Review
    {
        public string text { get; set; }
        public string date { get; set; }
        public string link { get; set; }
        public List<string> comments { get; set; }

        private string getReviewString()
        {
            try
            {
                string stringToReturn = "";
                stringToReturn += this.date.ToString() + '\t' + this.text + '\t' + this.link;
                foreach(string rc in comments)
                {
                    string stringToAdd = "";
                    stringToAdd = "\n\r" + rc;
                    stringToReturn += stringToAdd;
                }
                return stringToReturn.Replace("&nbsp;", " ");
            }
            catch
            {
                return "Unsuccessful building";
            }
        }

        private string getReviewStringOnlyTema()
        {
            try
            {                
                string stringToReturn = "";
                stringToReturn += this.link;
                foreach (string rc in comments)
                {
                    string stringToAdd = "";
                    stringToAdd = "\n\r" + rc;
                    stringToReturn += stringToAdd;
                }                
                return stringToReturn.Replace("&nbsp;", " ").Replace("&laquo;", "").Replace("&raquo;", "");
            }
            catch
            {
                return "Invalid link";
            }
        }

        public static void saveReviewsToFile(List<Review> listOfReviews)
        {
            using (StreamWriter sw = new StreamWriter("ParseResult.txt"))
            {
                foreach(Review review in listOfReviews)
                {                   
                    sw.WriteLine(review.getReviewStringOnlyTema());
                }
            }
        }

        public static void saveToJson(List<Review> listOfReviews)
        {
            var js = JsonConvert.SerializeObject(listOfReviews);
            using (StreamWriter sw = new StreamWriter("ParseResult.json"))
            {
                sw.WriteLine(js);
            }
        }
    }
}
