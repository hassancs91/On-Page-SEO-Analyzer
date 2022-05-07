using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeoAnalyzer
{
    internal class SeoAnalyzer
    {
        public string PageHtml { get; set; }
        public string WebUrl { get; set; }
        public long MainLoadTime { get; set; }

        public SeoAnalyzer(string url)
        {
            PageHtml = string.Empty;
            

            var watch = new Stopwatch();
            watch.Start();
            LoadPageHtml(url);
            watch.Stop();

            MainLoadTime = watch.ElapsedMilliseconds;

            WebUrl = url;
        }

        public void LoadPageHtml(string url)
        {
            try
            {
                var web = new HtmlWeb();
                var htmlDoc = web.Load(url);
                var html = htmlDoc.ParsedText.ToLower();
                PageHtml = html;
            }
            catch (Exception ex)
            {
                PageHtml = string.Empty;
            }

        }

        public bool CheckHtmlIfLoaded()
        {
            return PageHtml != string.Empty;
        }



        //Get Title //70 In Google Preview
        public string GetMetaTitle()
        {
            try
            {

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(PageHtml);
                var title = htmlDoc.DocumentNode.Descendants("title");
                return title == null ? "MISSING" : HttpUtility.HtmlDecode(title.FirstOrDefault()?.InnerText);

            }
            catch
            {
                return string.Empty;
            }

        }

        //Meta Description //160 Chars in Google Preview
        public string GetMetaDescription()
        {
            try
            {
                if (PageHtml == null)
                {
                    return "NOT_LOADED";
                }


                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(PageHtml);
                var metaTags = htmlDoc.DocumentNode.SelectNodes("//meta");
                if (metaTags != null)
                {
                    foreach (var sitetag in metaTags)
                    {
                        if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value == "description")
                        {
                            return HttpUtility.HtmlDecode(sitetag.Attributes["content"].Value);
                        }
                    }
                }


            }
            catch
            {

            }

            return string.Empty;
        }


        //Social Media Meta Tags
        //Facebook
        public OgGraph GetOpenGraphTags()
        {
            var openGraphTags = new OgGraph();
            try
            {
                if (PageHtml == null)
                {
                    return null;
                }


                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(PageHtml);
                var metaTags = htmlDoc.DocumentNode.SelectNodes("//meta");
                if (metaTags != null)
                {
                    foreach (var sitetag in metaTags)
                    {
                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["property"].Value.ToLower() == "og:title")
                        {
                            openGraphTags.title = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["property"].Value.ToLower() == "og:type")
                        {
                            openGraphTags.type = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["property"].Value.ToLower() == "og:url")
                        {
                            openGraphTags.url = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["property"].Value.ToLower() == "og:image")
                        {
                            openGraphTags.image = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["property"].Value.ToLower() == "og:site_name")
                        {
                            openGraphTags.site_name = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["property"].Value.ToLower() == "og:description")
                        {
                            openGraphTags.description = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["property"].Value.ToLower() == "og:locale")
                        {
                            openGraphTags.locale = sitetag.Attributes["content"].Value;
                        }



                    }
                }


            }
            catch
            {

            }

            return openGraphTags;
        }
        //Twitter
        public TwitterCard GetTwitterCard()
        {
            var twitterCard = new TwitterCard();
            try
            {
                if (PageHtml == null)
                {
                    return null;
                }


                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(PageHtml);
                var metaTags = htmlDoc.DocumentNode.SelectNodes("//meta");
                if (metaTags != null)
                {
                    foreach (var sitetag in metaTags)
                    {
                        if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value.ToLower() == "twitter:card")
                        {
                            twitterCard.card = sitetag.Attributes["content"].Value;

                        }
                        if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value.ToLower() == "twitter:site")
                        {
                            twitterCard.site = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value.ToLower() == "twitter:title")
                        {
                            twitterCard.title = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value.ToLower() == "twitter:description")
                        {
                            twitterCard.description = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value.ToLower() == "twitter:image")
                        {
                            twitterCard.image = sitetag.Attributes["content"].Value;
                        }
                    }

                }
            }


            catch
            {

            }

            return twitterCard;
        }


        //Get Headings

        //Get H1 Tags
        public List<string> GetH1Headers()
        {
            var headers = new List<string>();
            try
            {

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(PageHtml);
                var h2Tags = htmlDoc.DocumentNode.Descendants("h1");
                foreach (var tag in h2Tags)
                {
                    headers.Add(HttpUtility.HtmlDecode(tag.InnerText));
                }


            }
            catch
            {
                //
            }
            return headers;
        }
        //Get H2 Headers
        public List<string> GetH2Headers()
        {
            var headers = new List<string>();
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(PageHtml);
                var h2Tags = htmlDoc.DocumentNode.Descendants("h2");
                foreach (var tag in h2Tags)
                {
                    headers.Add(HttpUtility.HtmlDecode(tag.InnerText));
                }


                //  return h1Tag == null ? "MISSING" : HttpUtility.HtmlDecode(h1Tag.FirstOrDefault()?.InnerText);
            }
            catch
            {
                //
            }

            return headers;
        }
        //You can do the same for other heading tags!!!!


        //Robots.txt
        public string CheckRobotsTxt()
        {
            try
            {

                Uri myUri = new Uri(WebUrl);
                string host = myUri.Host;


                var testUrl = host;
                if (testUrl.EndsWith("/"))
                {
                    testUrl = WebUrl + "robots.txt";
                }
                else
                {
                    testUrl = WebUrl + "/robots.txt";
                }

                HttpClient client = new HttpClient();
                var checkingResponse = client.GetAsync(testUrl).Result;
                if (checkingResponse.IsSuccessStatusCode && checkingResponse.StatusCode == HttpStatusCode.OK)
                {
                    //load robots file
                    WebClient wc = new WebClient();
                    var page = wc.DownloadString(testUrl);


                    return page.ToString();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }


        //Get Anchor Tags
        public List<AnchorTag> GetAnchorTags()
        {
            var links = new List<AnchorTag>();
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(PageHtml);
                var anchorTags = htmlDoc.DocumentNode.Descendants("a");
                foreach (var tag in anchorTags)
                {
                    var link = new AnchorTag();

                    if (tag.Attributes["href"] != null)
                    {
                        link.href = tag.Attributes["href"].Value;

                    }
                    if (tag.Attributes["target"] != null)
                    {
                        link.target = tag.Attributes["target"].Value;

                    }
                    if (tag.Attributes["title"] != null)
                    {
                        link.title = tag.Attributes["title"].Value;

                    }
                    if (tag.Attributes["rel"] != null)
                    {
                        link.rel = tag.Attributes["rel"].Value;

                    }
                    if (tag.Attributes["title"] != null)
                    {
                        link.title = tag.Attributes["title"].Value;

                    }
                    if (tag.Attributes["name"] != null)
                    {
                        link.name = tag.Attributes["name"].Value;

                    }
                    if (tag.InnerText != null)
                    {
                        link.text = tag.InnerText;

                    }

                    if (link.href != null && !link.href.StartsWith("#"))
                    {
                        //check if external
                        Uri baseUri = new Uri(WebUrl);
                        var domain = baseUri.Host;

                        Uri baseUriLink = new Uri(link.href);
                        var domainLink = baseUriLink.Host;

                        if (domain.ToLower() == domainLink.ToLower())
                        {
                            link.type = "Internal";
                        }
                        else
                        {
                            link.type = "External";
                        }
                    }
                    else
                    {
                        link.type = "InPage";
                    }


                    links.Add(link);


                }



            }
            catch
            {
                //
            }

            return links;
        }


        //Get Image Tags //Add Format
        public List<ImageTag> GetImageTags()
        {
            var images = new List<ImageTag>();
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(PageHtml);
                var imgTags = htmlDoc.DocumentNode.Descendants("img");
                foreach (var tag in imgTags)
                {
                    var img = new ImageTag();

                    if (tag.Attributes["src"] != null)
                    {
                        img.src = tag.Attributes["src"].Value;

                        img.type = Path.GetExtension(img.src);


                    }
                    if (tag.Attributes["alt"] != null)
                    {

                        if (tag.Attributes["alt"].Value.Trim().Length == 0)
                        {
                            img.alt = "EMPTY";
                        }
                        else
                        {
                            img.alt = tag.Attributes["alt"].Value;

                        }
                    }
                    else
                    {
                        img.alt = "EMPTY";
                    }



                    images.Add(img);
                }
            }
            catch
            {
                //
            }

            return images;
        }


        //Favicon
        public string GetIcon()
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(PageHtml);
            var linkIcons = htmlDoc.DocumentNode.Descendants("link");

            foreach (var icon in linkIcons)
            {
                if (icon.Attributes["rel"] != null && icon.Attributes["rel"].Value.ToString().ToLower() == "icon")
                {
                    return icon.Attributes["href"].Value.ToString().ToLower();
                }

                if (icon.Attributes["rel"] != null && icon.Attributes["rel"].Value.ToString().ToLower() == "shortcut icon")
                {
                    return icon.Attributes["href"].Value.ToString().ToLower();
                }

                if (icon.Attributes["rel"] != null && icon.Attributes["rel"].Value.ToString().ToLower() == "apple-touch-icon")
                {
                    return icon.Attributes["href"].Value.ToString().ToLower();
                }

                if (icon.Attributes["rel"] != null && icon.Attributes["rel"].Value.ToString().ToLower() == "apple-touch-icon-precomposed")
                {
                    return icon.Attributes["href"].Value.ToString().ToLower();
                }

            }


            return string.Empty;

        }

        //Check Gzip Compression
        public bool isGzip()
        {

            var httpRequest = (HttpWebRequest)WebRequest.Create(WebUrl);
            httpRequest.Headers["Accept-Encoding"] = "gzip";

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            if (httpResponse.ContentEncoding != null && httpResponse.ContentEncoding.ToLower() == "gzip")
            {
                return true;
            }
            else
            {

                return false;
            }
        }


        //Doc Type
        public bool GetDocType()
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(PageHtml);
            var html = htmlDoc.ParsedText;
            return html.ToLower().Contains("!doctype");

        }


        //Load Time Using Selenium and Web Automation !! Make Sure To Check Your Chrome Version Compatibility
        public PageLoad PageLoadTime()
        {
            var pLoad = new PageLoad();

            var option = new ChromeOptions();
            option.AddArguments("--headless");
            option.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
            IWebDriver driver = new ChromeDriver("E:\\Chrome\\", option);
            driver.Navigate().GoToUrl(WebUrl);


            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var navigationStart = (Int64)js.ExecuteScript("return window.performance.timing.navigationStart");
            var responseStart = (Int64)js.ExecuteScript("return window.performance.timing.responseStart");
            var domComplete = (Int64)js.ExecuteScript("return window.performance.timing.domComplete");

            var backendPerformance_calc = responseStart - navigationStart;
            var frontendPerformance_calc = domComplete - responseStart;
            pLoad.backendPerformance = Convert.ToDouble(backendPerformance_calc) / 1000;
            pLoad.frontendPerformance = Convert.ToDouble(frontendPerformance_calc) / 1000;

            return pLoad;

            

        }


        




        //Classes
        #region "Classes"

        public class OgGraph
        {
            public string title { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public string image { get; set; }
            public string site_name { get; set; }
            public string description { get; set; }
            public string locale { get; set; }

        }

        public class TwitterCard
        {
            public string title { get; set; }
            public string card { get; set; }
            public string site { get; set; }
            public string image { get; set; }
            public string description { get; set; }
        }

        public class AnchorTag
        {

            public string href { get; set; }
            public string target { get; set; }
            public string text { get; set; }
            public string title { get; set; }
            public string rel { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }

        public class ImageTag
        {
            public string src { get; set; }
            public string alt { get; set; }
            public string type { get; set; }
        }

        public class PageLoad
        {
            public double backendPerformance { get; set; }
            public double frontendPerformance { get; set; }
        }



        #endregion




    }
}
