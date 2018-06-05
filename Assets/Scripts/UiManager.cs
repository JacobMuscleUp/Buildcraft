using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager Ins { get; private set; }

    [SerializeField] GameObject blockButtonGroup;
    [SerializeField] GameObject sceneSlotButtonGroup;
    [SerializeField] GameObject prefabBlockButton;
    [SerializeField] Text rowText;
    [SerializeField] Text colText;
    [SerializeField] Text blockManipulationModeText;
    [SerializeField] Slider rowSlider;
    [SerializeField] Slider colSlider;
    [SerializeField] Slider blockSeparationSlider;
    [SerializeField] Slider blockManipulationModeSlider;
    [SerializeField] Button saveSceneButton;
    [SerializeField] Button cloningModeButton;

    [SerializeField] float buttonNormalColorAlpha;
    [SerializeField] float buttonPressedColorAlpha;

    public bool OnUi { get; private set; }
    public bool OnUiDrag { get; private set; }

    Stack<Image> blockButtonImages = new Stack<Image>();
    Dictionary<GameObject, Button> blockToButtonMap = new Dictionary<GameObject, Button>();

    void Awake()
    {
        Ins = this;

        GameEventSignals.OnMapResize += OnMapResize;
        InputSignals.OnUiEnter += OnUiEnter;
        InputSignals.OnUiExit += OnUiExit;
        InputSignals.OnUiDragBegin += OnUiDragBegin;
        InputSignals.OnUiDragEnd += OnUiDragEnd;

        rowSlider.onValueChanged.AddListener((float _val) => {
            var newRowCount = (int)_val;
            if (newRowCount != Map.Ins.rowCount)
                GameEventSignals.DoMapResize(newRowCount, Map.Ins.colCount);
        });
        colSlider.onValueChanged.AddListener((float _val) => {
            var newColCount = (int)_val;
            if (newColCount != Map.Ins.colCount)
                GameEventSignals.DoMapResize(Map.Ins.rowCount, newColCount);
        });
        blockManipulationModeSlider.onValueChanged.AddListener((float _val) => {
            blockManipulationModeText.text = Utility.GetBlockManipulationModeText(BlockManager.Ins.Mode = (BlockManager.EnumMode)_val);
            Block.UnhighlightAll();
        });
        cloningModeButton.onClick.AddListener(() => {
            BlockManager.Ins.CloningMode = (BlockManager.EnumCloningMode)(((int)BlockManager.Ins.CloningMode + 1) % 2);
        });
    }

    void Start()
    {
        var prefabBlocks = BlockManager.Ins.prefabBlocks;
        for (int i = 0; i < prefabBlocks.Length; ++i) {
            var blockButtonGameObject = Instantiate(prefabBlockButton);
            var blockButtonButton = blockButtonGameObject.GetComponent<Button>();
            blockButtonGameObject.transform.SetParent(blockButtonGroup.transform);
            blockButtonImages.Push(blockButtonGameObject.GetComponent<Image>());
            blockButtonImages.Peek().color = prefabBlocks[i].GetComponent<MeshRenderer>().sharedMaterial.color;
            blockToButtonMap[prefabBlocks[i]] = blockButtonButton;
            var j = i;
            blockButtonButton.onClick.AddListener(() => {
                foreach (var blockButtonImage in blockButtonImages)
                    blockButtonImage.color = Utility.ModifyAlpha(blockButtonImage.color, buttonNormalColorAlpha);
                var currentImage = blockButtonGameObject.GetComponent<Image>();
                currentImage.color = Utility.ModifyAlpha(currentImage.color, buttonPressedColorAlpha);
                BlockManager.Ins.CurrentBlockIndex = j;
            });
        }

        rowSlider.value = Map.Ins.rowCount;
        colSlider.value = Map.Ins.colCount;
        
        blockManipulationModeSlider.value = blockManipulationModeSlider.maxValue = int.MaxValue;
        blockManipulationModeSlider.value = (float)BlockManager.Ins.Mode;
        blockManipulationModeSlider.maxValue = Enum.GetValues(typeof(BlockManager.EnumMode)).Length - 1;

        blockSeparationSlider.value = Map.Ins.blockSeparation;
        rowText.text = string.Format("{0} {1}", rowSlider.value, (rowSlider.value != 1) ? "rows" : "row");
        colText.text = string.Format("{0} {1}", colSlider.value, (rowSlider.value != 1) ? "columns" : "column");
        blockToButtonMap[BlockManager.Ins.prefabBlocks[BlockManager.Ins.CurrentBlockIndex]].onClick.Invoke();
        saveSceneButton.onClick.Invoke();

        blockSeparationSlider.onValueChanged.AddListener((float _val) => { GameEventSignals.DoMapRescale(_val); });
    }

    void OnDestroy()
    {
        GameEventSignals.OnMapResize -= OnMapResize;
        InputSignals.OnUiEnter -= OnUiEnter;
        InputSignals.OnUiExit -= OnUiExit;
        InputSignals.OnUiDragBegin -= OnUiDragBegin;
        InputSignals.OnUiDragEnd -= OnUiDragEnd;

        rowSlider.onValueChanged.RemoveAllListeners();
        colSlider.onValueChanged.RemoveAllListeners();
        blockManipulationModeSlider.onValueChanged.RemoveAllListeners();
        blockSeparationSlider.onValueChanged.RemoveAllListeners();
    }

    void OnMapResize(int _rowCount, int _colCount)
    {
        rowText.text = string.Format("{0} {1}", _rowCount, (_rowCount != 1) ? "rows" : "row");
        colText.text = string.Format("{0} {1}", _colCount, (_colCount != 1) ? "columns" : "column");
    }

    void OnUiEnter()
    {
        OnUi = true;
    }

    void OnUiExit()
    {
        OnUi = false;
    }

    void OnUiDragBegin()
    {
        OnUiDrag = true;
    }

    void OnUiDragEnd()
    {
        OnUiDrag = false;
    }

    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void OnSaveSceneButtonClick()
    {
        sceneSlotButtonGroup.SetActive(!sceneSlotButtonGroup.activeSelf);
        var saveSceneButtonImage = saveSceneButton.GetComponent<Image>();
        saveSceneButtonImage.color = Utility.ModifyAlpha(saveSceneButtonImage.color
            , (!sceneSlotButtonGroup.activeSelf) ? buttonPressedColorAlpha : buttonNormalColorAlpha);
    }

    public void OnSceneSlotButtonClick(int _slotIndex)
    {
        SceneSavingManager.SetSlotIndex(_slotIndex);
        SceneSavingManager.Save();
        saveSceneButton.onClick.Invoke();
    }
}
