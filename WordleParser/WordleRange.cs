namespace WordleParser;

public class WordleRange
{
    public string Range { get; set; }
    public string FamilyMember { get; set; }
    public int Played { get; set; }
    public double Average { get; set; }
    public double AverageDistanceFromAverageScore { get; set; }

    public override string ToString()
    {
        return Range.PadRight(10) +
               FamilyMember.PadRight(20) +
               Played.ToString().PadLeft(4) + "   " +
               Average.ToString("F3").PadLeft(6) + "   " +
               AverageDistanceFromAverageScore.ToString("F3").PadLeft(6);
    }
}