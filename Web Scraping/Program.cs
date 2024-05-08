using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Scraping
{
    class Program
    {
        //----Scrapping article------
        //https://www.zenrows.com/blog/web-scraping-c-sharp

        //----Test scrap site-----
        //https://scrapeme.live/shop

        static void Main(string[] args)
        {

            AgilityPackScraper scraper = new AgilityPackScraper("https://scrapeme.live/shop/page/1/");
            scraper.ScrapAllPages(48);


            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
