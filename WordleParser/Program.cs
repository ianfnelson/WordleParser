if (args is not [{ } filePath])
{
    Console.WriteLine("Usage: Provide a valid file path as an argument.");
    return;
}

if (!File.Exists(filePath))
{
    Console.WriteLine($"Error: The file '{filePath}' does not exist.");
    return;
}

foreach (var range in WordleParser.WordleParser.Parse(filePath))
{
    Console.WriteLine(range.Name);
    Console.WriteLine();

    foreach (var entry in range.Entries)
    {
        Console.WriteLine(entry);
    }
    
    Console.WriteLine();
}
