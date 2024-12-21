using System.Text.RegularExpressions;

namespace WordleParser;

public class WordleParser
{
    public static IEnumerable<WordleRange> Parse(string filepath)
    {
        var scores = GetScores(filepath).ToList();

        var years = scores.GroupBy(x => x.Date.Year.ToString());
        foreach (var range in BuildRanges(years))
        {
            yield return range;
        }

        var months = scores.GroupBy(x => $"{x.Date.Year}-{x.Date.Month}");
        foreach (var range in BuildRanges(months))
        {
            yield return range;
        }
    }

    private static IEnumerable<WordleScore> GetScores(string filepath)
    {
        var scores = ParseScores(filepath)
            .DistinctBy(x => new {x.Date, x.FamilyMember})
            .ToList();
        
        var dailyAverages = scores
            .GroupBy(x => x.Date)
            .ToDictionary(
                g => g.Key, 
                g => g.Average(x => x.Score)
            );

        foreach (var score in scores)
        {
            score.DistanceFromAverageScore = score.Score - dailyAverages[score.Date];
            yield return score;
        }
    }
    
    private static IEnumerable<WordleScore> ParseScores(string filepath)
    {
        var regex = new Regex(@"^\[.{20}\] (.*): Wordle (\d{3}|\d{1},\d{3}).{1,5}(.)/6.*$");
        
        foreach (var line in File.ReadLines(filepath))
        {
            var match = regex.Match(line);

            if (!match.Success) continue;
            
            var score = match.Groups[3].Value;
            var wordleScore = new WordleScore
            {
                FamilyMember = match.Groups[1].Value,
                Wordle = int.Parse(match.Groups[2].Value.Replace(",","")),
                Score = score == "X" ? 7 : int.Parse(score)
            };

            yield return wordleScore;
        }
    }

    private static IEnumerable<WordleRange> BuildRanges(IEnumerable<IGrouping<string, WordleScore>> scoreGroups)
    {
        foreach (var scoreGroup in scoreGroups)
        {
            var familyMembers = scoreGroup
                .GroupBy(x => x.FamilyMember)
                .OrderBy(x => x.Average(y => y.DistanceFromAverageScore));
            
            foreach (var familyMember in familyMembers)
            {
                yield return new WordleRange
                {
                    Average = familyMember.Average(x => x.Score),
                    AverageDistanceFromAverageScore = familyMember.Average(x => x.DistanceFromAverageScore),
                    FamilyMember = familyMember.Key,
                    Range = scoreGroup.Key,
                    Played = familyMember.Count()
                };
            }
        }
    }
}

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
               Played.ToString().PadRight(4) +
               Average.ToString("F3").PadRight(6) +
               AverageDistanceFromAverageScore.ToString("F3").PadRight(6);
    }
}

public class WordleScore
{
    private readonly DateOnly _wordleEpoch = new(2021, 6, 19);
    
    public DateOnly Date => _wordleEpoch.AddDays(Wordle);

    public required string FamilyMember { get; set; } 
    public int Wordle { get; set; }
    public int Score { get; set; }
    
    public double DistanceFromAverageScore { get; set; }
}