using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public int Height { get; private set; }
    public int Index { get; private set; }

    static Dictionary<int, Dictionary<int, Dictionary<int, bool>>> occupiedMap = new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>();
    static Dictionary<int, Dictionary<int, Dictionary<int, Block>>> all = new Dictionary<int, Dictionary<int, Dictionary<int, Block>>>();
    public static Dictionary<int, Dictionary<int, Dictionary<int, Block>>> All { get { return all; } }

    static int count = 0;
    public static int Count {
        get { return count; }
        private set {
            count = value;
        }
    }

    public static Block SelectedBlock { get; set; }
    public static LinkedList<Block> highlightedBlocks = new LinkedList<Block>();

    float highlightedAlpha = 0.2f;
    float normalAlpha = 1.0f;

    public void Initialize(int _row, int _col, int _height, int _index)
    {
        Row = _row;
        Col = _col;
        Height = _height;
        Index = _index;
        
        if (!occupiedMap.ContainsKey(Row))
            occupiedMap[Row] = new Dictionary<int, Dictionary<int, bool>>();
        if (!occupiedMap[Row].ContainsKey(Col))
            occupiedMap[Row][Col] = new Dictionary<int, bool>();
        occupiedMap[Row][Col][Height] = true;
        if (!All.ContainsKey(Row))
            All[Row] = new Dictionary<int, Dictionary<int, Block>>();
        if (!All[Row].ContainsKey(Col))
            All[Row][Col] = new Dictionary<int, Block>();
        All[Row][Col][Height] = this;

        Count = Count + 1;
    }

    public void GetDestroyed()
    {
        Destroy(gameObject);

        occupiedMap[Row][Col].Remove(Height);
        if (occupiedMap[Row][Col].Count == 0)
            occupiedMap[Row].Remove(Col);
        if (occupiedMap[Row].Count == 0)
            occupiedMap.Remove(Row);

        All[Row][Col].Remove(Height);
        if (All[Row][Col].Count == 0)
            All[Row].Remove(Col);
        if (All[Row].Count == 0)
            All.Remove(Row);

        highlightedBlocks.Remove(this);

        Count = Count - 1;
    }

    // NOT used when containers of Block objects are iterated over
    public void GetUnhighlighted(bool _flag = true)
    {
        Utility.ModifyAlpha(GetComponent<MeshRenderer>(), normalAlpha);
        if (_flag)
            highlightedBlocks.Remove(this);
    }

    public void GetHighlighted()
    {
        Utility.ModifyAlpha(GetComponent<MeshRenderer>(), highlightedAlpha);
        highlightedBlocks.AddLast(this);
    }

    public static int UnhighlightAll()
    {
        var count = 0;
        for (; highlightedBlocks.Count > 0; highlightedBlocks.RemoveLast(), ++count)
            highlightedBlocks.Last.Value.GetUnhighlighted(false);
        return count;
    }

    public static bool Occupied(int _row, int _col, int _height)
    {
        return occupiedMap.ContainsKey(_row) 
            && occupiedMap[_row].ContainsKey(_col) 
            && occupiedMap[_row][_col].ContainsKey(_height);
    }

    public static void RestoreStaticFields()
    {
        occupiedMap.Clear();
        all.Clear();
        count = 0;
        SelectedBlock = null;
        highlightedBlocks.Clear();
    }
}
