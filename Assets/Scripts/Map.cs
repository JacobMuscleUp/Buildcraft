using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    public static Map Ins { get; private set; }

    public int rowCount;
    public int colCount;
    public Vector3 centerPos;
    public float blockSeparation;
    public GameObject prefabBlock;

    public Vector3 TopLeftCornerPos { get; private set; }
    public Vector3 BlockExtents { get; private set; }

    void Awake()
    {
        Ins = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEventSignals.OnMapResize += OnMapResize;
        GameEventSignals.OnMapRescale += OnMapRescale;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEventSignals.OnMapResize -= OnMapResize;
        GameEventSignals.OnMapRescale -= OnMapRescale;
    }

    void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        Block.RestoreStaticFields();

        if (_scene.buildIndex != 1)
            SceneSavingManager.LoadRowAndColumn();

        TopLeftCornerPos = new Vector3(
            (2 * centerPos.x - (colCount - 1) * blockSeparation) / 2
            , centerPos.y
            , (2 * centerPos.z - (rowCount - 1) * blockSeparation) / 2);
        BlockExtents = prefabBlock.GetComponent<MeshRenderer>().bounds.extents * 2;

        GetGenerated(0, rowCount, 0, colCount);

        if (_scene.buildIndex != 1)
            SceneSavingManager.LoadBlocks();
    }

    void OnMapResize(int _rowCount, int _colCount)
    {
        if (rowCount < _rowCount)
            GetGenerated(rowCount, _rowCount, 0, colCount);
        else if (rowCount > _rowCount)
            GetRemoved(_rowCount, rowCount, 0, colCount);
        rowCount = _rowCount;

        if (colCount < _colCount)
            GetGenerated(0, rowCount, colCount, _colCount);
        else if (colCount > _colCount)
            GetRemoved(0, rowCount, _colCount, colCount);
        colCount = _colCount;
    }

    void OnMapRescale(float _blockSeparation)
    {
        blockSeparation = _blockSeparation;

        for (int row = 0; row < rowCount; ++row)
            for (int col = 0; col < colCount; ++col)
                foreach (var elem in Block.All[row][col])
                    elem.Value.transform.position 
                        = TopLeftCornerPos + new Vector3(col, elem.Key, row) * blockSeparation;
    }

    public void GetGenerated(int _rowBegin, int _rowEnd, int _colBegin, int _colEnd)
    {
        for (int row = _rowBegin; row < _rowEnd; ++row) {
            for (int col = _colBegin; col < _colEnd; ++col) {
                var block = Instantiate(prefabBlock
                    , TopLeftCornerPos + new Vector3(col, 0, row) * blockSeparation
                    , prefabBlock.transform.rotation).GetComponent<Block>();
                block.Initialize(row, col, 0, 0);
            }
        }
    }

    void GetRemoved(int _rowBegin, int _rowEnd, int _colBegin, int _colEnd)
    {
        var removedBlocks = new Stack<Block>();
        for (int row = _rowBegin; row < _rowEnd; ++row) 
            for (int col = _colBegin; col < _colEnd; ++col) 
                foreach (var elem in Block.All[row][col])
                    removedBlocks.Push(elem.Value);
        while (removedBlocks.Count > 0)
            removedBlocks.Pop().GetDestroyed();
    }
}
