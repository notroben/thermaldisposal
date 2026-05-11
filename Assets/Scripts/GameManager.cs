using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Day Settings")]
    public static int globalDay = 1;
    public int currentDay = 1;
    public int dailyQuota = 10;
    public DayData[] dayConfigs;
    public DayData CurrentDayData => (dayConfigs != null && currentDay <= dayConfigs.Length && currentDay > 0) ? dayConfigs[currentDay - 1] : null;

    [Header("Testing & Debug")]
    [Tooltip("Check this box to force the game to start on the Current Day number below")]
    public bool forceStartingDay = false;

    [Header("Facility Systems")]
    public GameObject scannerTool;
    public TextMeshProUGUI clipboardHeaderText;
    public ConveyorBelt conveyorBelt;
    public GameObject ironPoker;
    public GameObject taskListContainer;
    public GameObject exitDoorObject;

    [Header("Day 8 (Execution)")]
    public GameObject alarmSystem;
    public GameObject gameOverCanvas;
    public TextMeshProUGUI gameOverReasonText;

    [Header("Progress")]
    public int bagsBurnedToday = 0;

    [Header("Honesty Tracking")]
    public int physicalWeighedCount = 0;
    public int uiCheckedCount = 0;
    public int uiCrossedOutCount = 0;

    [Header("Game Over Tracking")]
    public static string globalRuleBreakReason = "";
    public bool fatalRuleBroken = false;
    public string ruleBreakReason = "";

    [Header("Day 5 Unlocks")]
    public GameObject day5ExtraTaskRow;

    void Start()
    {
        if (forceStartingDay)
        {
            globalDay = currentDay;
            forceStartingDay = false; // turn off to not break normal scene loading
        }
        else
        {
            currentDay = globalDay;
        }

        ruleBreakReason = globalRuleBreakReason;
        if (ruleBreakReason != "") fatalRuleBroken = true;

        SetupMorningShift();
    }

    void SetupMorningShift()
    {
        DayData config = CurrentDayData;
        if (config == null)
        {
            Debug.LogWarning("No DayData assigned for current day: " + currentDay);
            return;
        }

        if (conveyorBelt != null) conveyorBelt.isBroken = config.conveyorBroken;

        dailyQuota = config.dailyQuota;
        if (day5ExtraTaskRow != null) day5ExtraTaskRow.SetActive(config.showDay5ExtraTask);
        if (ironPoker != null) ironPoker.SetActive(config.unlockIronPoker);
        if (taskListContainer != null) taskListContainer.SetActive(config.showTaskList);
        if (exitDoorObject != null) exitDoorObject.SetActive(config.showExitDoor);

        if (config.isExecutionDay)
        {
            if (scannerTool != null) scannerTool.SetActive(false);
            if (clipboardHeaderText != null) clipboardHeaderText.transform.parent.gameObject.SetActive(false);

            StartCoroutine(ExecutionRoutine());
            return;
        }

        if (scannerTool != null) scannerTool.SetActive(config.unlockScanner);
        if (clipboardHeaderText != null) clipboardHeaderText.text = config.directiveText;
    }

    IEnumerator ExecutionRoutine()
    {
        if (alarmSystem != null) alarmSystem.SetActive(true);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);

        Debug.Log("AUDIO: alarm playing");

        yield return new WaitForSeconds(8f);

        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        if (gameOverReasonText != null) gameOverReasonText.text = ruleBreakReason;

        Time.timeScale = 0f;
    }

    public void AddBurnedBag(TrashBag_data bagData)
    {
        if (!bagData.hasBeenWeighed) TriggerFutureGameOver("You incinerated un-weighed material.");
        bagsBurnedToday++;
        Debug.Log("Bags Burned: " + bagsBurnedToday + " / " + dailyQuota);
    }

    public void TriggerFutureGameOver(string reason)
    {
        fatalRuleBroken = true;
        ruleBreakReason = reason;
        globalRuleBreakReason = reason;
        Debug.Log("FATAL ERROR LOGGED: " + reason);
    }

    public void VerifyFurnaceHonesty(int bagsAboutToBurn)
    {
        int totalBagsAfterBurn = bagsBurnedToday + bagsAboutToBurn;
        if (uiCheckedCount < totalBagsAfterBurn)
        {
            TriggerFutureGameOver("Missing Documentation: Material incinerated prior to logging checklist.");
        }
    }
    public void TriggerTrueEnding()
    {
        StartCoroutine(TrueEndingRoutine());
    }
    IEnumerator TrueEndingRoutine()
    {
        Debug.Log("AUDIO: furnace roar");

        yield return new WaitForSeconds(2f);

        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        if (gameOverReasonText != null)
        {
            gameOverReasonText.text = "THERMAL DISPOSAL COMPLETE.\nThank you for your service to The Company.";
        }

        Time.timeScale = 0f;
    }
}