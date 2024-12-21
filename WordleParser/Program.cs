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

foreach (var line in WordleParser.WordleParser.Parse(filePath))
{
    Console.WriteLine(line);
}
