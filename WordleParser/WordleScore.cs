namespace WordleParser;

public class WordleScore
{
    private readonly DateOnly _wordleEpoch = new(2021, 6, 19);
    
    public DateOnly Date => _wordleEpoch.AddDays(Wordle);

    public required string FamilyMember { get; set; } 
    public int Wordle { get; set; }
    public int Score { get; set; }
}