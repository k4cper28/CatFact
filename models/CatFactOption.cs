namespace CatFact.models;

public class CatFactOption
{
    public const string SectionName = "CatFactSettings";

    public string ApiUrl { get; set; } = string.Empty;
    public string FilePath { get; set; } = "cat_facts.txt";

}

