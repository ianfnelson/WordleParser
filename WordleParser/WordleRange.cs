namespace WordleParser;

public class WordleRange
{
    public string Name { get; set; }
    
    public List<WordleRangeEntry> Entries { get; set; }
}

public class WordleRangeEntry
{
    public string Rank { get; set; }
    public string FamilyMember { get; set; }
    public int Played { get; set; }
    public double Average { get; set; }
    public double AverageDistanceFromAverageScore { get; set; }

    public override string ToString()
    {
        return Rank.PadRight(3) +
               FamilyMember.PadRight(16) +
               Played.ToString().PadLeft(4) + "   " +
               Average.ToString("F3").PadLeft(6) + "   " +
               AverageDistanceFromAverageScore.ToString("F3").PadLeft(6);
    }
}