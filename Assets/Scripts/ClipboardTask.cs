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
            GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.OutOfSequence);
        }

        gameManager.uiCheckedCount++;

        if (gameManager.uiCheckedCount > gameManager.physicalWeighedCount)
        {
            GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.FalsifiedWeigh);
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
            GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.OutOfSequence);
        }

        isBurned = true;
        gameManager.uiCrossedOutCount++;

        if (gameManager.uiCrossedOutCount > gameManager.bagsBurnedToday)
        {
            GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.FalsifiedBurn);
        }

        taskText.text = "<s>" + originalText + "</s>";
        taskText.color = new Color(0.4f, 0.4f, 0.4f);
    }
}