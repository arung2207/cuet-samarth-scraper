using CuetSamarthScraper;
using CuetSamarthScraper.Models;
using Fizzler.Systems.HtmlAgilityPack;

var stopwatch = new System.Diagnostics.Stopwatch();
stopwatch.Start();

const string CuetSamarthUrlRoot = "https://cuetug.ntaonline.in"; //2024
const string CuetSamarthUniversityUrl = $"{CuetSamarthUrlRoot}/universities";

List<University> Universities = new();

List<UniversityType> UniversityTypes = new()
{
    new UniversityType{Id="CU", Name="Central University"},
    new UniversityType{Id="STATE", Name="State University"},
    new UniversityType{Id="DEEMED", Name="Deemed University"},
    new UniversityType{Id="PRIVATE", Name="Private University"},
    new UniversityType{Id="GOVT", Name="Government University"}
};

var homePageDoc = await Helper.GetReponseAsync(CuetSamarthUniversityUrl);
if (homePageDoc == null)
{
    Console.WriteLine($"Unable to read home page: {CuetSamarthUniversityUrl}");
    Console.ReadKey();
    return;
}

Console.WriteLine($"Read home page: {CuetSamarthUniversityUrl}");

//fetch initial universities related details from home page
var uTypeDivs = homePageDoc.GetElementbyId("searchItems").QuerySelectorAll("div.card.card-white.ovflw.mt-4");

int t = 0;
foreach (var uType in uTypeDivs)
{
    var univs = uType.QuerySelectorAll("div.col-md-4");
    foreach (var univ in univs)
    {
        //save details
        Universities.Add(new University
        {
            EligibilityUrl = $"{univ.QuerySelector("a").GetAttributeValue("href", "")}",
            Type = UniversityTypes[t],
            Name = univ.QuerySelector("h3").GetDirectInnerText(),
        });
    }
    t++;
}

Console.WriteLine("Fetching individual university programs...");

//get university specific programs by navigating to each eligibility url
int count = 0;
foreach (var university in Universities)
{
    var universityDoc = await Helper.GetReponseAsync(university.EligibilityUrl);
    if (universityDoc != null)
    {
        var ucard = universityDoc.DocumentNode.QuerySelector(".card-body");
        //university.WebsiteUrl = ucard.QuerySelector("a").GetAttributeValue("href", "");
        university.Programs = Helper.GetProgramsFromTable(ucard.QuerySelector(".table").InnerHtml);
    }

    Console.WriteLine($"{++count} of {Universities.Count} - fetched {university.Programs.Count} programs from '{university.Name}' data...");
}

var CuetCsvFileName = $"cuetdata-{DateTime.Now:yyyyMMMdd}.csv";
Helper.SaveToFile(Helper.PrepareUniversityData(Universities), CuetCsvFileName);

Console.WriteLine($"Finished saving data in {Path.GetFullPath(CuetCsvFileName)}");

stopwatch.Stop();
Console.WriteLine("Time taken : {0}. Press O|o to open file, any other key to close.", stopwatch.Elapsed);

var key = Console.ReadKey();
if (key.KeyChar is 'o' or 'O')
    Helper.OpenFile(Path.GetFullPath(CuetCsvFileName));