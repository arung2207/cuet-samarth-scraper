using CsvHelper;
using CuetSamarthScraper.Models;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;

namespace CuetSamarthScraper;

public static class Helper
{
    /// <summary>
    /// Fetches the HTMLDocument from the url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<HtmlDocument?> GetReponseAsync(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            try
            {
                using var client = new HttpClient();

                var msg = new HttpRequestMessage(HttpMethod.Get, url);
                msg.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.84 Safari/537.36");
                msg.Headers.Add("Accept", "text/html; charset=utf-8");

                var res = await client.SendAsync(msg);

                var s = await res.Content.ReadAsStringAsync();
                if (s != null)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(s);
                    return doc;
                }
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Parses the programs HTML table to extract program data
    /// </summary>
    /// <param name="tableHtml"></param>
    /// <returns></returns>
    public static List<UniversityProgram> GetProgramsFromTable(string tableHtml)
    {
        var p =  new List<UniversityProgram>();

        var doc = new HtmlDocument();
        doc.LoadHtml(tableHtml);

        var myTableRows = doc.DocumentNode.Descendants("tr");
        foreach (var tr in myTableRows)
        {
            var cells = tr.Descendants("td").Select(td => td.InnerText).ToList();
            if (cells != null && cells.Count == 5) //sometimes count is not 5 due to malformed html (noticed table within table)
            {
                p.Add(new UniversityProgram
                {
                    Degree = cells[0],
                    CourseOffered = cells[1],
                    MappedSubjects = cells[2],
                    Eligibility = cells[3].Sanitize(),
                });
            }
        }
        
        return p;
    }

    /// <summary>
    /// Prepares the Program model from Universities information
    /// </summary>
    /// <param name="universities"></param>
    /// <returns></returns>
    public static List<ProgramCsvModel>? PrepareUniversityData(List<University> universities)
    {
        if (universities == null || !universities.Any())
            return null;

        var programs = new List<ProgramCsvModel>();
        foreach (var u in universities)
        {
            foreach (var p in u.Programs)
            {
                programs.Add(new ProgramCsvModel
                {
                    Type = u.Type.Id,
                    Name = u.Name,
                    EligibilityUrl = u.EligibilityUrl,
                    WebsiteUrl = u.WebsiteUrl,
                    Degree = p.Degree,
                    CourseOffered = p.CourseOffered,
                    MappedSubjects = p.MappedSubjects,
                    Eligibility = p.Eligibility,
                    Remarks = p.Remarks
                });
            }

        }
        return programs;
    }

    /// <summary>
    /// Saves the Programs information into a CSV file
    /// </summary>
    /// <param name="programs"></param>
    /// <param name="fileName"></param>
    public static void SaveToFile(List<ProgramCsvModel>? programs, string fileName)
    {
        if (programs == null || !programs.Any() || string.IsNullOrEmpty(fileName))
            return;

        using var writer = new StreamWriter(fileName);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(programs);
    }

    /// <summary>
    /// Opens file with default program
    /// </summary>
    /// <param name="file"></param>
    public static void OpenFile(string file)
    {
        if (File.Exists(file))
        {
            using Process fileopener = new();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = $"\"{file}\"";
            fileopener.Start();
        }
    }

    /// <summary>
    /// Removes the non-ascii characters from string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Sanitize(this string input)
    {
        return Encoding.ASCII.GetString(
            Encoding.Convert(
                Encoding.UTF8,
                Encoding.GetEncoding(
                    Encoding.ASCII.EncodingName,
                    new EncoderReplacementFallback(string.Empty),
                    new DecoderExceptionFallback()
                    ),
                Encoding.UTF8.GetBytes(HttpUtility.HtmlDecode(input))
            )
        );
    }

}
