using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace SeoAnalyzer
{
    internal class SeoAnalyzer
    {
        public string PageHtml { get; set; }
        public string WebUrl { get; set; }
        public long MainLoadTime { get; set; }

        public SeoAnalyzer(string url, string pageHtml)
        {
            var watch = new Stopwatch();
            watch.Start();
            LoadPageHtml(url);
            watch.Stop();

            MainLoadTime = watch.ElapsedMilliseconds;

            WebUrl = url;
            PageHtml = pageHtml;
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

        //Google Search Results Preview //HTML Based on UI

        //Social Media Meta Tags
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

        //You can do the same for other heading tags


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


    }
}
