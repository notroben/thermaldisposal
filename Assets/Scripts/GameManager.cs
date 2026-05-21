using UnityEngine;
using UnityEngine.SceneManagement;
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStaticState()
    {
        globalDay = 1;
        globalRuleBreakReason = "";
        godMode = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitGlobalSettings()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    [Header("Testing & Debug")]
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
    public static bool godMode = false;
    public bool fatalRuleBroken = false;
    public string ruleBreakReason = "";

    [Header("Day 5 Unlocks")]
    public GameObject day5ExtraTaskRow;

    void Awake()
    {
        ServiceLocator.RegisterGameManager(this);
    }

    void OnEnable()
    {
        GameEvents.OnTriggerGameOver += TriggerFutureGameOver;
    }

    void OnDisable()
    {
        GameEvents.OnTriggerGameOver -= TriggerFutureGameOver;
    }

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
        if (config == null) return;
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

        yield return new WaitForSeconds(10f);

        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        if (alarmSystem != null) alarmSystem.SetActive(false);
        if (gameOverReasonText != null) gameOverReasonText.text = ruleBreakReason;
        if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlayGlobalSFX("GameOver");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void AddBurnedBag(TrashBag_data bagData)
    {
        if (!bagData.hasBeenWeighed) TriggerFutureGameOver(RuleBreak.UnweighedBurn);
        bagsBurnedToday++;
        Debug.Log("Bags Burned: " + bagsBurnedToday + " / " + dailyQuota);
    }

    public void TriggerFutureGameOver(string reason)
    {
        if (godMode)
        {
            Debug.Log("GOD MODE: Ignored infraction - " + reason);
            return;
        }
        fatalRuleBroken = true;
        ruleBreakReason = reason;
        globalRuleBreakReason = reason;
        Debug.Log("HR SYSTEM: Infraction logged. Employee marked for Day 8 execution. Reason: " + reason);
    }

    public void VerifyFurnaceHonesty(int bagsAboutToBurn)
    {
        int totalBagsAfterBurn = bagsBurnedToday + bagsAboutToBurn;
        if (uiCheckedCount < totalBagsAfterBurn) TriggerFutureGameOver(RuleBreak.UndocumentedBurn);
    }
    public void TriggerTrueEnding()
    {
        StartCoroutine(TrueEndingRoutine());
    }
    IEnumerator TrueEndingRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlayGlobalSFX("FurnaceRoar");

        yield return new WaitForSeconds(0.5f);

        if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlayGlobalSFX("EndScreen");
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        if (gameOverReasonText != null) gameOverReasonText.text = "THERMAL DISPOSAL COMPLETE.\nYour service to The Company has been concluded.";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        globalDay = 1;
        globalRuleBreakReason = "";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}