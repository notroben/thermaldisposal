using UnityEngine;
using TMPro;

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

    [Header("Progress")]
    public int bagsBurnedToday = 0;

    [Header("Honesty Tracking")]
    public int physicalWeighedCount = 0;
    public int uiCheckedCount = 0;
    public int uiCrossedOutCount = 0;

    [Header("Game Over Tracking")]
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

        SetupMorningShift();
    }

    void SetupMorningShift()
    {
        if (conveyorBelt != null) conveyorBelt.isBroken = false;

        dailyQuota = (currentDay == 5) ? 11 : 10;
        if (day5ExtraTaskRow != null)
        {
            day5ExtraTaskRow.SetActive(currentDay == 5);
        }

        if (currentDay == 1)
        {
            if (scannerTool != null) scannerTool.SetActive(false);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 1 TASK:\nIncinerate 10 Trash Bags.";
        }
        else if (currentDay == 2)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 2 TASK:\nIncinerate 10 bags with NO METAL. Use the SCANNER.";
        }
        else if (currentDay == 3)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 3 TASK:\nIncinerate 10 bags. CONVEYOR is currently under maintenance.";
            if (conveyorBelt != null) conveyorBelt.isBroken = true;
        }
        else if (currentDay == 4)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 4 TASK:\nIncinerate 10 bags. IMPORTANT: Ignore acoustics, they are just trapped gases escaping.";
        }
        else if (currentDay == 5)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 5 TASK:\nIncinerate 10 bags. IMPORTANT: Move excess trash from OVER_CAPACITY bags into 1 empty bag.";
        }
        else if (currentDay == 6)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 6 TASK:\nIncinerate 10 bags. IMPORTANT: Clear out unburned trash in the furnace with the IRON POKER.";
        }
        else if (currentDay >= 7)
        {
            if (scannerTool != null) scannerTool.SetActive(true);
            if (clipboardHeaderText != null) clipboardHeaderText.text = "DAY 7 TASK:\nIncinerate 1 trash.";
        }
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
}