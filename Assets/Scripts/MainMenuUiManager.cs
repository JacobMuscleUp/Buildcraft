using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] GameObject loadScenePageGroup;
    [SerializeField] GameObject mainPageGroup;
    [SerializeField] GameObject sceneSlotButtonGroup;
    [SerializeField] Button removeSceneButton;

    [SerializeField] float buttonNormalColorAlpha;
    [SerializeField] float buttonPressedColorAlpha;

    Button[] sceneSlotButtons;

    bool inRemoveSceneMode = false;

    void Awake()
    {
        mainPageGroup.SetActive(true);
        loadScenePageGroup.SetActive(false);

        sceneSlotButtons = sceneSlotButtonGroup.GetComponentsInChildren<Button>();
        var directoryPath = string.Format("{0}/Resources/Scene Files", Application.dataPath);

        foreach (var button in sceneSlotButtons)
            button.interactable = false;
        if (Directory.Exists(directoryPath)) {
            for (int i = 0; i < sceneSlotButtons.Length; ++i)
                if (File.Exists(string.Format("{0}/{1}.csv", directoryPath, i)))
                    sceneSlotButtons[i].interactable = true;
        }
        
    }

    public void OnNewSceneButtonClick()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void OnLoadSceneButtonClick()
    {
        mainPageGroup.SetActive(false);
        loadScenePageGroup.SetActive(true);
    }

    public void OnBackToMainButtonClick()
    {
        mainPageGroup.SetActive(true);
        loadScenePageGroup.SetActive(false);
    }

    public void OnSceneSlotButtonClick(int _slotIndex)
    {
        SceneSavingManager.SetSlotIndex(_slotIndex);
        if (inRemoveSceneMode) {
            sceneSlotButtons[_slotIndex].interactable = false;
            SceneSavingManager.Delete();
        }
        else
            SceneManager.LoadSceneAsync(2);
    }

    public void OnRemoveSceneButtonClick()
    {
        var removeSceneButtonImage = removeSceneButton.GetComponent<Image>();
        inRemoveSceneMode = !inRemoveSceneMode;
        removeSceneButtonImage.color = Utility.ModifyAlpha(removeSceneButtonImage.color
            , (inRemoveSceneMode) ? buttonPressedColorAlpha : buttonNormalColorAlpha);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
