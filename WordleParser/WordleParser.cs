using System.Text.RegularExpressions;

namespace WordleParser;

public class WordleParser
{
    public static IEnumerable<WordleRange> Parse(string filepath)
    {
        var scores = GetScores(filepath).ToList();

        var years = scores.GroupBy(x => x.Date.Year.ToString())
            .OrderByDescending(x => x.Key)
            .Take(2);
        
        foreach (var year in years)
        {
            yield return BuildRange(year.Key, year);
        }

        var months = scores
            .GroupBy(x => $"{x.Date.Year}-{x.Date.Month:00}")
            .OrderByDescending(x => x.Key)
            .Take(2);
        
        foreach (var month in months)
        {
            var monthName = month.First().Date.ToString("MMMM");
            yield return BuildRange(monthName, month);
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

    private static WordleRange BuildRange(string name, IGrouping<string, WordleScore> scoreGroup)
    {
        var range = new WordleRange { Name = name };

        var entries = scoreGroup
            .GroupBy(x => x.FamilyMember)
            .OrderBy(x => x.Average(y => y.Score))
            .Select(familyMember => new WordleRangeEntry
            {
                Average = familyMember.Average(x => x.Score),
                AverageDistanceFromAverageScore = familyMember.Average(x => x.DistanceFromAverageScore),
                FamilyMember = familyMember.Key,
                Played = familyMember.Count()
            }).ToList();

        AssignRanks(entries);

        range.Entries = entries;

        return range;
    }

    private static void AssignRanks(List<WordleRangeEntry> entries)
    {
        var currentRank = 1;

        for (var i = 0; i < entries.Count; i++)
        {
            if (i > 0 && 
                Math.Abs(entries[i].Average - entries[i - 1].Average) < 0.0001D && 
                Math.Abs(entries[i].AverageDistanceFromAverageScore - entries[i - 1].AverageDistanceFromAverageScore) < 0.0001D)
            {
                entries[i].Rank = $"{entries[i - 1].Rank}";
            }
            else
            {
                entries[i].Rank = $"{currentRank}.";
            }

            currentRank++;
        }
    }
}