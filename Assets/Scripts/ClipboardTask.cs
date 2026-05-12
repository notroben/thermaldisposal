using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClipboardTask : MonoBehaviour
{
    [Header("Sequence Setting")]
    public int taskIndex;

    [Header("UI References")]
    public TextMeshProUGUI taskText;
    public Toggle checkbox;

    private GameManager gameManager;
    private bool isBurned = false;
    private string originalText;

    void Start()
    {
        gameManager = ServiceLocator.GameManager;

        if (taskText != null)
        {
            originalText = taskText.text;
        }
    }

    public void OnCheckboxChanged(bool isChecked)
    {
        if (!isChecked) return;

        if (taskIndex != gameManager.uiCheckedCount)
        {
            GameEvents.OnTriggerGameOver?.Invoke("Sloppy Documentation: Paperwork logged out of chronological sequence.");
        }

        gameManager.uiCheckedCount++;

        if (gameManager.uiCheckedCount > gameManager.physicalWeighedCount)
        {
            GameEvents.OnTriggerGameOver?.Invoke("Falsified Records: Marked material as weighed prior to validation.");
        }

        if (checkbox != null)
        {
            checkbox.interactable = false;
        }
    }

    public void ToggleBurnedStatus()
    {
        if (isBurned) return;

        if (taskIndex != gameManager.uiCrossedOutCount)
        {
            GameEvents.OnTriggerGameOver?.Invoke("Sloppy Documentation: Incineration logged out of chronological sequence.");
        }

        isBurned = true;
        gameManager.uiCrossedOutCount++;

        if (gameManager.uiCrossedOutCount > gameManager.bagsBurnedToday)
        {
            GameEvents.OnTriggerGameOver?.Invoke("Falsified Records: Marked material as incinerated prior to thermal disposal.");
        }

        taskText.text = "<s>" + originalText + "</s>";
        taskText.color = new Color(0.4f, 0.4f, 0.4f);
    }
}