using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveLoadPanel : MonoBehaviour
{
    public enum Mode { Save, Load }
    public enum ConfirmAction { Save, Load, Delete }

    [Header("Panel UI")]
    public Image panelTitleImage;

    [Header("Save Slots")]
    public SaveSlotUI[] saveSlots;

    [Header("Confirmation Prompt")]
    public GameObject confirmPanel;
    public Image confirmPromptImage;
    public Button confirmYesButton;
    public Button confirmNoButton;

    [Header("Prompt Images (Designed Assets)")]
    public Sprite promptSaveSprite;
    public Sprite promptLoadSprite;
    public Sprite promptDeleteSprite;

    [HideInInspector] public Mode currentMode = Mode.Load;

    private int pendingSlotIndex = -1;
    private ConfirmAction pendingAction;

    void Awake()
    {
        foreach (SaveSlotUI slot in saveSlots)
        {
            if (slot != null) slot.Initialize(this);
        }

        if (confirmYesButton != null) confirmYesButton.onClick.AddListener(OnConfirmYes);
        if (confirmNoButton != null) confirmNoButton.onClick.AddListener(OnConfirmNo);
    }

    public void OpenPanel(Mode mode)
    {
        currentMode = mode;
        gameObject.SetActive(true);

        if (confirmPanel != null) confirmPanel.SetActive(false);

        RefreshAllSlots();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }

    public void RefreshAllSlots()
    {
        foreach (SaveSlotUI slot in saveSlots)
        {
            if (slot != null) slot.RefreshDisplay();
        }
    }

    public void ShowConfirmation(int slotIndex, ConfirmAction action)
    {
        pendingSlotIndex = slotIndex;
        pendingAction = action;

        if (confirmPromptImage != null)
        {
            switch (action)
            {
                case ConfirmAction.Save: confirmPromptImage.sprite = promptSaveSprite; break;
                case ConfirmAction.Load: confirmPromptImage.sprite = promptLoadSprite; break;
                case ConfirmAction.Delete: confirmPromptImage.sprite = promptDeleteSprite; break;
            }
        }

        if (confirmPanel != null) confirmPanel.SetActive(true);
    }

    void OnConfirmYes()
    {
        if (pendingSlotIndex < 0) return;

        switch (pendingAction)
        {
            case ConfirmAction.Save:
                SaveManager.SaveGame(pendingSlotIndex);
                if (confirmPanel != null) confirmPanel.SetActive(false);
                RefreshAllSlots();
                break;

            case ConfirmAction.Load:
                SaveManager.LoadGame(pendingSlotIndex);
                break;

            case ConfirmAction.Delete:
                SaveManager.DeleteSave(pendingSlotIndex);
                if (confirmPanel != null) confirmPanel.SetActive(false);
                RefreshAllSlots();
                break;
        }

        pendingSlotIndex = -1;
    }

    void OnConfirmNo()
    {
        pendingSlotIndex = -1;
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }
}
