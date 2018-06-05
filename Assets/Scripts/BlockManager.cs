using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockManager : MonoBehaviour
{
    class BlockInfo
    {
        public int row, col, height, index;

        public BlockInfo(int _row, int _col, int _height, int _index)
        {
            row = _row; col = _col; height = _height; index = _index;
        }
    }

    public enum EnumMode { placing, cloning }
    public enum EnumCloningMode { alongRow, alongCol }

    public static BlockManager Ins { get; private set; }

    public GameObject[] prefabBlocks;
    public int CurrentBlockIndex { get; set; }
    [SerializeField] float blockPlacingTime;
    float blockPlacingTimer;
    public float GapWidth { get; private set; }

    public EnumMode Mode { get; set; }
    public EnumCloningMode CloningMode { get; set; }

    void Awake()
    {
        Ins = this;

        GameEventSignals.OnMapRescale += OnMapRescale;
        
        CurrentBlockIndex = 0;
        blockPlacingTimer = blockPlacingTime;
        Mode = EnumMode.placing;
        CloningMode = EnumCloningMode.alongCol;
    }

    void Start()
    {
        GapWidth = Map.Ins.blockSeparation - Map.Ins.BlockExtents[0];
    }

    void Update()
    {
        if (UiManager.Ins.OnUi || UiManager.Ins.OnUiDrag) return;

        if (Mode == EnumMode.placing)
            HandleBlockPlacing();
        else if (Mode == EnumMode.cloning) 
            HandleBlockCloning(CloningMode);
    }

    void HandleBlockPlacing()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit)) {
            var target = raycastHit.collider.gameObject;
            if (target.CompareTag("Block")) {
                var block = target.GetComponent<Block>();

                if (VirtualAxisManager.GetAxis("Fire1")) {
                    blockPlacingTimer += Time.deltaTime;
                    if (blockPlacingTimer > blockPlacingTime)
                        blockPlacingTimer = 0;
                    else
                        goto Proc0;

                    var targetBlock = target.GetComponent<Block>();
                    var downRay = Utility.GetBlockDownwardRay(targetBlock, raycastHit.point, GapWidth);
                    
                    if (Physics.Raycast(downRay, out raycastHit)) {
                        var target0 = raycastHit.collider.gameObject;
                        if (target0.CompareTag("Block")) {
                            var targetBlock0 = target0.GetComponent<Block>();
                            int row, col, height;
                            if (targetBlock.Row == targetBlock0.Row && targetBlock.Col == targetBlock0.Col) {
                                if (targetBlock.Height == targetBlock0.Height) {
                                    row = targetBlock.Row;
                                    col = targetBlock.Col;
                                    height = targetBlock.Height + 1;
                                }
                                else {
                                    row = targetBlock.Row;
                                    col = targetBlock.Col;
                                    height = targetBlock.Height - 1;
                                }
                            }
                            else  {
                                row = targetBlock0.Row;
                                col = targetBlock0.Col;
                                height = targetBlock.Height;
                            }

                            if (Block.Occupied(row, col, height)) goto Proc0;
                            target = PlaceBlock(row, col, height, CurrentBlockIndex).gameObject;
                        }// if (target0.CompareTag("Block"))
                    }// if (Physics.Raycast(downRay, out raycastHit))
                }// if (VirtualAxisManager.GetAxis("Fire1"))
                else if (VirtualAxisManager.GetAxisUp("Fire2")) {
                    if (CameraManager.Ins.Rotating || target.GetComponent<Block>().Height == 0) goto Proc0;
                    block.GetDestroyed();
                }
                else
                    blockPlacingTimer = blockPlacingTime;

            Proc0:
                if (Block.SelectedBlock == block)
                    return;
                if (Block.SelectedBlock != null)
                    Block.SelectedBlock.GetUnhighlighted();
                if (target != null)
                    (Block.SelectedBlock = target.GetComponent<Block>()).GetHighlighted();
            }// if (target.CompareTag("Block"))
            else {
                DeselectAllBlocks();
            }
        }
        else {
            DeselectAllBlocks();
        }
    }

    void HandleBlockCloning(EnumCloningMode _mode = EnumCloningMode.alongCol)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit)) {
            var target = raycastHit.collider.gameObject;
            if (target.CompareTag("Block")) {
                var block = target.GetComponent<Block>();
                if (VirtualAxisManager.GetAxisUp("Fire1"))
                    CloneMapAnotherHalf(block, _mode);

                if (Block.SelectedBlock != null && block == Block.SelectedBlock) return;
                Block.SelectedBlock = block;
                
                Block.UnhighlightAll();
                HighlightMapHalf(block, _mode);
            }// if (target.CompareTag("Block"))
            else {
                Block.SelectedBlock = null;
                Block.UnhighlightAll();
            }
        }
        else {
            Block.SelectedBlock = null;
            Block.UnhighlightAll();
        }
    }

    void OnDestroy()
    {
        GameEventSignals.OnMapRescale -= OnMapRescale;
    }

    void OnMapRescale(float _blockSeparation)
    {
        GapWidth = Map.Ins.blockSeparation - Map.Ins.BlockExtents[0];
    }

    void DeselectAllBlocks()
    {
        if (Block.SelectedBlock != null) {
            Block.SelectedBlock.GetUnhighlighted();
            Block.SelectedBlock = null;
        }
    }

    void HighlightMapHalf(Block _currentBlock, EnumCloningMode _mode = EnumCloningMode.alongCol)
    {
        Action<Block> BlockManip0 = (_block) => { }, BlockManip1 = (_block) => { };

        if (_mode == EnumCloningMode.alongCol) {
            if (_currentBlock.Col < Map.Ins.colCount / 2) {
                BlockManip0 = (_block) => { _block.GetUnhighlighted(); };
                BlockManip1 = (_block) => { _block.GetHighlighted(); };
            }
            else if (_currentBlock.Col > (Map.Ins.colCount - 1) / 2) {
                BlockManip0 = (_block) => { _block.GetHighlighted(); };
                BlockManip1 = (_block) => { _block.GetUnhighlighted(); };
            }

            for (int row = Map.Ins.rowCount - 1; row >= 0; --row) {
                for (int col = (Map.Ins.colCount + 1) / 2; col < Map.Ins.colCount; ++col)
                    BlockManip0(Block.All[row][col][0]);
                for (int col = Map.Ins.colCount / 2 - 1; col >= 0; --col)
                    BlockManip1(Block.All[row][col][0]);
            }
        }
        else if (_mode == EnumCloningMode.alongRow) {
            if (_currentBlock.Row < Map.Ins.rowCount / 2) {
                BlockManip0 = (_block) => { _block.GetUnhighlighted(); };
                BlockManip1 = (_block) => { _block.GetHighlighted(); };
            }
            else if (_currentBlock.Row > (Map.Ins.rowCount - 1) / 2) {
                BlockManip0 = (_block) => { _block.GetHighlighted(); };
                BlockManip1 = (_block) => { _block.GetUnhighlighted(); };
            }

            for (int col = Map.Ins.colCount - 1; col >= 0; --col) {
                for (int row = (Map.Ins.rowCount + 1) / 2; row < Map.Ins.rowCount; ++row)
                    BlockManip0(Block.All[row][col][0]);
                for (int row = Map.Ins.rowCount / 2 - 1; row >= 0; --row)
                    BlockManip1(Block.All[row][col][0]);
            }
        }
    }

    void CloneMapAnotherHalf(Block _currentBlock, EnumCloningMode _mode = EnumCloningMode.alongCol)
    {
        var clonedBlockInfos = new Stack<BlockInfo>();
        var removedBlocks = new Stack<Block>();
        Func<int, int, bool> Predicate0;

        if (_mode == EnumCloningMode.alongCol) {
            if (_currentBlock.Col < Map.Ins.colCount / 2)
                Predicate0 = (int a, int b) => { return a >= (b + 1) / 2; };
            else
                Predicate0 = (int a, int b) => { return a < b / 2; };

            foreach (var elem in Block.All) {
                var row = elem.Key;
                foreach (var elem0 in elem.Value) {
                    var col = elem0.Key;
                    foreach (var elem1 in elem0.Value) {
                        var height = elem1.Key;
                        var symmetricBlockCol = Map.Ins.colCount - 1 - col;

                        if (height == 0) continue;
                        if (Predicate0(col, Map.Ins.colCount)) {
                            if (!Block.Occupied(row, symmetricBlockCol, height)
                                || Block.All[row][col][height].Index != Block.All[row][symmetricBlockCol][height].Index)
                                removedBlocks.Push(elem1.Value);
                            continue;
                        }

                        if (!Block.Occupied(row, symmetricBlockCol, height)
                            || Block.All[row][col][height].Index != Block.All[row][symmetricBlockCol][height].Index)
                            clonedBlockInfos.Push(new BlockInfo(row, symmetricBlockCol, height, elem1.Value.Index));
                    }
                }
            }
        }// if (_mode == EnumCloningMode.alongCol)
        else if (_mode == EnumCloningMode.alongRow) {
            if (_currentBlock.Row < Map.Ins.rowCount / 2)
                Predicate0 = (int a, int b) => { return a >= (b + 1) / 2; };
            else
                Predicate0 = (int a, int b) => { return a < b / 2; };

            foreach (var elem in Block.All) {
                var row = elem.Key;
                foreach (var elem0 in elem.Value) {
                    var col = elem0.Key;
                    foreach (var elem1 in elem0.Value) {
                        var height = elem1.Key;
                        var symmetricBlockRow = Map.Ins.rowCount - 1 - row;

                        if (height == 0) continue;
                        if (Predicate0(row, Map.Ins.rowCount)) {
                            if (!Block.Occupied(symmetricBlockRow, col, height)
                                || Block.All[row][col][height].Index != Block.All[symmetricBlockRow][col][height].Index)
                                removedBlocks.Push(elem1.Value);
                            continue;
                        }

                        if (!Block.Occupied(symmetricBlockRow, col, height)
                            || Block.All[row][col][height].Index != Block.All[symmetricBlockRow][col][height].Index)
                            clonedBlockInfos.Push(new BlockInfo(symmetricBlockRow, col, height, elem1.Value.Index));
                    }
                }
            }
        }// else if (_mode == EnumCloningMode.alongRow)

        while (removedBlocks.Count > 0)
            removedBlocks.Pop().GetDestroyed();
        while (clonedBlockInfos.Count > 0) {
            var blockInfo = clonedBlockInfos.Pop();
            PlaceBlock(blockInfo.row, blockInfo.col, blockInfo.height, blockInfo.index);
        }
    }

    public Block PlaceBlock(int _row, int _col, int _height, int _blockIndex)
    {
        var newBlock = Instantiate(prefabBlocks[_blockIndex]).GetComponent<Block>();
        newBlock.Initialize(_row, _col, _height, _blockIndex);
        newBlock.transform.position = Map.Ins.TopLeftCornerPos
            + new Vector3(newBlock.Col * Map.Ins.blockSeparation, newBlock.Height * Map.Ins.blockSeparation, newBlock.Row * Map.Ins.blockSeparation);
        return newBlock;
    }
}
