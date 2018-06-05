using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public static class SceneSavingManager
{
    static CsvParser parser;
    static int index;

    public static void SetSlotIndex(int _index)
    {
        index = _index;
    }

    public static void Save()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(string.Format("{0},{1},{2}\n", Map.Ins.rowCount, Map.Ins.colCount, Map.Ins.blockSeparation));
        foreach (var elem0 in Block.All) {
            foreach (var elem1 in elem0.Value) {
                foreach (var elem2 in elem1.Value) {
                    var block = elem2.Value;
                    if (block.Height == 0) continue;
                    stringBuilder.Append(string.Format("{0},{1},{2},{3}\n", block.Row, block.Col, block.Height, block.Index));
                }
            }
        }
        var directoryPath = string.Format("{0}/Resources/Scene Files", Application.dataPath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        File.WriteAllText(string.Format("{0}/Resources/Scene Files/{1}.csv", Application.dataPath, index), stringBuilder.ToString());
    }

    public static void Delete()
    {
        File.Delete(string.Format("{0}/Resources/Scene Files/{1}.csv", Application.dataPath, index));
    }

    public static void LoadRowAndColumn()
    {
        var fileText = File.ReadAllText(string.Format("{0}/Resources/Scene Files/{1}.csv", Application.dataPath, index));
        parser = new CsvParser();
        parser.Parse(fileText);
        
        Map.Ins.rowCount = int.Parse(parser.ProcessedData[0][0]);
        Map.Ins.colCount = int.Parse(parser.ProcessedData[0][1]);
        Map.Ins.blockSeparation = float.Parse(parser.ProcessedData[0][2]);
    }

    public static void LoadBlocks()
    {
        for (int i = parser.ProcessedData.Count - 1; i > 0; --i) {
            var currentLine = parser.ProcessedData[i];
            BlockManager.Ins.PlaceBlock(int.Parse(currentLine[0]), int.Parse(currentLine[1])
                , int.Parse(currentLine[2]), int.Parse(currentLine[3]));
        }
    }
}
