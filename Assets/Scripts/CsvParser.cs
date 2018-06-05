using System.Collections.Generic;

public class CsvParser
{
    public List<List<string>> ProcessedData { get; private set; }

    /// <summary>
    /// Field Delimiter Char
    /// </summary>
    public char Delimiter { get; set; }

    /// <summary>
    /// Line Delimiter Char
    /// </summary>
    public char LineDelimiter { get; set; }

    /// <summary>
    /// Quote Char
    /// </summary>
    public char Quote { get; set; }

    public CsvParser()
    {
        ProcessedData = new List<List<string>>();
    }

    public void Parse(string _fileText)
    {
        ProcessedData.Clear();

        var lines = _fileText.Split('\n');
        var rowCount = lines.Length;
        if (lines[lines.Length - 1] == "") --rowCount;

        for (int i = 0; i < rowCount; ++i) {
            ProcessedData.Add(new List<string>());
            var fields = lines[i].Split(',');
            for (int j = 0; j < fields.Length; ++j)
                ProcessedData[i].Add(fields[j]);
        }
    }
}
