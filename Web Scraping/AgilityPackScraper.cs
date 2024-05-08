using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using CsvHelper;
using System.IO;
using System.Globalization;

namespace Web_Scraping
{
    public class AgilityPackScraper
    {
        private HtmlWeb web;
        private string firstPageUrl;

        private List<PokemonProduct> pokemonProducts;

        public AgilityPackScraper(string url)
        {
            firstPageUrl = url;

            web = new HtmlWeb();
            //setting a global User-Agent header in HAP to avoid block
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

            pokemonProducts = new List<PokemonProduct>();
        }

        public void ScrapAllPages(int pageLimit)
        {
            // the list of pages discovered during the crawling task 
            var pagesDiscovered = new List<string> { firstPageUrl };

            // the list of pages that remains to be scraped 
            var pagesToScrape = new Queue<string>();

            // initializing the list with firstPageToScrape 
            pagesToScrape.Enqueue(firstPageUrl);

            // current crawling iteration 
            int i = 1;

            // until there are no pages to scrape or limit is hit 
            while (pagesToScrape.Count != 0 && i <= pageLimit)
            {
                // extracting the current page to scrape from the queue 
                var currentPage = pagesToScrape.Dequeue();

                // loading the page 
                var currentDocument = web.Load(currentPage);

                // selecting the list of pagination HTML elements 
                var paginationHTMLElements = currentDocument.DocumentNode.QuerySelectorAll("a.page-numbers");

                // to avoid visiting a page twice 
                foreach (var paginationHTMLElement in paginationHTMLElements)
                {
                    // extracting the current pagination URL 
                    var newPaginationLink = paginationHTMLElement.Attributes["href"].Value;

                    // if the page discovered is new 
                    if (!pagesDiscovered.Contains(newPaginationLink))
                    {
                        // if the page discovered needs to be scraped 
                        if (!pagesToScrape.Contains(newPaginationLink))
                        {
                            pagesToScrape.Enqueue(newPaginationLink);
                        }
                        pagesDiscovered.Add(newPaginationLink);
                    }
                }

                // scraping logic... 
                Console.WriteLine("Scrapping page: {0}", currentPage);
                FaceData(currentDocument);

                // incrementing the crawling counter 
                i++;
            }

            //write into csv file
            WriteCSV();
        }

        private void FaceData(HtmlDocument currentDocument)
        {
            var productHTMLElements = currentDocument.DocumentNode.QuerySelectorAll("li.product");

            // iterating over the list of product elements 
            foreach (var productHTMLElement in productHTMLElements)
            {
                var url = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("a").Attributes["href"].Value);
                var image = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img").Attributes["src"].Value);
                var name = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h2").InnerText);
                var price = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector(".price").InnerText);

                // instancing a new PokemonProduct object 
                var pokemonProduct = new PokemonProduct() { Url = url, Image = image, Name = name, Price = price };

                // adding the object containing the scraped data to the list 
                pokemonProducts.Add(pokemonProduct);
            }
            
        }

        private void WriteCSV()
        {
            // initializing the CSV output file and CSV writter
            using (var writer = new StreamWriter("pokemon-products.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(pokemonProducts);
            }
        }
 
    }
}
