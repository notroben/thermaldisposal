using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI slotTitleText;
    public TextMeshProUGUI slotDateText;
    public RawImage thumbnailImage;
    public Button slotButton;
    public Button deleteButton;

    [Header("Slot Config")]
    public int slotIndex = 1;

    private SaveLoadPanel parentPanel;
    private bool isOccupied = false;

    public void Initialize(SaveLoadPanel panel)
    {
        parentPanel = panel;

        if (slotButton != null) slotButton.onClick.AddListener(OnSlotClicked);
        if (deleteButton != null) deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    public void RefreshDisplay()
    {
        SaveData data = SaveManager.GetSaveData(slotIndex);
        isOccupied = data != null;

        if (isOccupied)
        {
            if (slotTitleText != null) slotTitleText.text = $"GAME SAVE {slotIndex:D2}";
            if (slotDateText != null) slotDateText.text = $"Day {data.currentDay} - {data.saveTimestamp}";

            Texture2D screenshot = SaveManager.LoadScreenshot(slotIndex);
            if (screenshot != null && thumbnailImage != null)
            {
                thumbnailImage.texture = screenshot;
                thumbnailImage.gameObject.SetActive(true);
            }
            if (deleteButton != null) deleteButton.interactable = true;
        }
        else
        {
            if (slotTitleText != null) slotTitleText.text = "EMPTY SAVE SLOT";
            if (slotDateText != null) slotDateText.text = "-";

            if (thumbnailImage != null) thumbnailImage.gameObject.SetActive(false);
            if (deleteButton != null) deleteButton.interactable = false;
        }
    }

    void OnSlotClicked()
    {
        if (parentPanel == null) return;

        if (parentPanel.currentMode == SaveLoadPanel.Mode.Save)
        {
            parentPanel.ShowConfirmation(slotIndex, SaveLoadPanel.ConfirmAction.Save);
        }
        else
        {
            if (isOccupied) parentPanel.ShowConfirmation(slotIndex, SaveLoadPanel.ConfirmAction.Load);
        }
    }

    void OnDeleteClicked()
    {
        if (parentPanel == null || !isOccupied) return;
        parentPanel.ShowConfirmation(slotIndex, SaveLoadPanel.ConfirmAction.Delete);
    }
}
