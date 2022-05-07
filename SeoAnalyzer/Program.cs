// See https://aka.ms/new-console-template for more information
Console.WriteLine("Welcome To The SEO Analyzer Tool");

Console.WriteLine("Enter URL To Analyze: ");

var url = Console.ReadLine();

if (url != null)
{
    var seoAnalyzer = new SeoAnalyzer.SeoAnalyzer(url);
    var isLoaded = seoAnalyzer.CheckHtmlIfLoaded();

    if (isLoaded)
    {
        Console.WriteLine("Page Loaded...Analysis Started");

        var pageTitle = seoAnalyzer.GetMetaTitle();
        Console.WriteLine("Meta Title: " + pageTitle);

        var metaDescription = seoAnalyzer.GetMetaDescription();
        Console.WriteLine("Meta Description: " + metaDescription);



    }
    else
    {
        Console.WriteLine("Page Not Loaded. Please Check Your URL");
    }

    



    Console.WriteLine("");
    Console.WriteLine("Done");
    Console.ReadLine();
}




