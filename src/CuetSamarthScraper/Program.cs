using CuetSamarthScraper;
using CuetSamarthScraper.Models;
using Fizzler.Systems.HtmlAgilityPack;

var stopwatch = new System.Diagnostics.Stopwatch();
stopwatch.Start();

const string CuetSamarthUrlRoot = "https://cuet.samarth.ac.in";
const string CuetSamarthUniversityUrl = $"{CuetSamarthUrlRoot}/index.php/app/info/universities";
const string CuetCsvFileName = "cuetdata.csv";

List<University> Universities = new();

List<UniversityType> UniversityTypes = new()
{
    new UniversityType{Id="CU", Name="Central University"},
    new UniversityType{Id="STATE", Name="State University"},
    new UniversityType{Id="DEEMED", Name="Deemed University"},
    new UniversityType{Id="PRIVATE", Name="Private University"}
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
foreach (var utype in UniversityTypes) //iterate via unversity types
{
    var ut = homePageDoc.GetElementbyId(utype.Id);

    var univDivs = ut.QuerySelectorAll(".min-h-100");

    foreach (var univDiv in univDivs)
    {
        //save details
        Universities.Add(new University
        {
            EligibilityUrl = $"{CuetSamarthUrlRoot}{univDiv.QuerySelector("a").GetAttributeValue("href", "")}",
            Type = utype,
            Name = univDiv.QuerySelector(".card").GetAttributeValue("title", ""),
        });
    }
}

Console.WriteLine("Fetching individual university programs...");

//get university specific programs by navigating to each eligibility url
int count = 0;
foreach (var university in Universities)
{
    var universityDoc = await Helper.GetReponseAsync(university.EligibilityUrl);
    if (universityDoc != null)
    {
        var ucard = universityDoc.DocumentNode.QuerySelector(".card");
        university.WebsiteUrl = ucard.QuerySelector("a").GetAttributeValue("href", "");
        university.Programs = Helper.GetProgramsFromTable(ucard.QuerySelector(".table-eligibility").InnerHtml);
    }

    Console.WriteLine($"{++count} of {Universities.Count} - fetched {university.Programs.Count} programs from '{university.Name}' data...");
}

Helper.SaveToFile(Helper.PrepareUniversityData(Universities), CuetCsvFileName);

Console.WriteLine($"Finished saving data in {Path.GetFullPath(CuetCsvFileName)}");

stopwatch.Stop();
Console.WriteLine("Time taken : {0}. Press O|o to open file, any other key to close.", stopwatch.Elapsed);

var key = Console.ReadKey();
if (key.KeyChar == 'o' || key.KeyChar == 'O')
    Helper.OpenFile(Path.GetFullPath(CuetCsvFileName));