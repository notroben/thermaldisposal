using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Day Settings")]
    public static int globalDay = 1;
    public int currentDay = 1;
    public int dailyQuota = 10;

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
        if (conveyorBelt != null) conveyorBelt.isBroken = false;

        dailyQuota = (currentDay == 5) ? 11 : 10;
        if (day5ExtraTaskRow != null) day5ExtraTaskRow.SetActive(currentDay == 5);
        if (ironPoker != null) ironPoker.SetActive(currentDay >= 6);
        if (taskListContainer != null) taskListContainer.SetActive(currentDay < 7);
        if (exitDoorObject != null) exitDoorObject.SetActive(currentDay < 7);

        if (currentDay == 8)
        {
            if (scannerTool != null) scannerTool.SetActive(false);
            if (clipboardHeaderText != null) clipboardHeaderText.transform.parent.gameObject.SetActive(false);

            StartCoroutine(ExecutionRoutine());
            return;
        }

        if (currentDay == 1)
        {
            if (scannerTool != null) scannerTool.SetActive(false);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 1 DIRECTIVE:\nProcess 10 standard disposal units. Accurate weight logging is mandatory.";
        }
        else if (currentDay == 2)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 2 DIRECTIVE:\nProcess 10 disposal units. Use SCANNER to ensure zero metallic contaminants prior to thermal destruction.";
        }
        else if (currentDay == 3)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 3 DIRECTIVE:\nProcess 10 disposal units. Automated delivery system is offline. Manual retrieval from the chute is required.";
            if (conveyorBelt != null) conveyorBelt.isBroken = true;
        }
        else if (currentDay == 4)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 4 DIRECTIVE:\nProcess 10 disposal units. NOTE: Acoustic anomalies during combustion are strictly expanding air pockets. Do not report them.";
        }
        else if (currentDay == 5)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 5 DIRECTIVE:\nProcess 10 disposal units. NOTE: Manually extract excess mass from OVER_CAPACITY units and consolidate into the overflow unit.";
        }
        else if (currentDay == 6)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 6 DIRECTIVE:\nProcess 10 disposal units. Suboptimal combustion detected. Pulverize all carbonized remains with the IRON POKER between incineration cycles.";
        }
        else if (currentDay == 7)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 7 DIRECTIVE:\nProcess the final unit.";
        }
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