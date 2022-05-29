namespace CuetSamarthScraper.Models;

public class University
{
    public University()
    {
        Programs = new List<UniversityProgram>();
    }

    public string? EligibilityUrl { get; set; }
    public UniversityType Type { get; set; }
    public string? Name { get; set; }
    public string? WebsiteUrl { get; set; }
    public List<UniversityProgram> Programs { get; set; }
}
