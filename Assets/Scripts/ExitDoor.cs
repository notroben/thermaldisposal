using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [Header("Connections")]
    public GameManager gameManager;
    public Image fadeScreen;
    public Image holdIndicator;

    [Header("Hold Settings")]
    public float requiredHoldTime = 2f;
    private float currentHoldTime = 0f;
    private bool isPlayerLooking = false;

    [Header("Fade Settings")]
    public float fadeSpeed = 1f;
    private bool isFading = false;

    void Start()
    {
        if (fadeScreen != null)
        {
            fadeScreen.color = new Color(0, 0, 0, 1f);
            StartCoroutine(FadeInRoutine());
        }
    }

    void Update()
    {
        if (isFading) return;

        if (isPlayerLooking && Input.GetMouseButton(0))
        {
            currentHoldTime += Time.deltaTime;
            float holdProgress = currentHoldTime / requiredHoldTime;

            if (holdIndicator != null) holdIndicator.fillAmount = holdProgress;
            if (currentHoldTime >= requiredHoldTime)
            {
                isFading = true;
                StartCoroutine(EndShiftRoutine());
            }
        }
        else
        {
            currentHoldTime = 0f;
            if (holdIndicator != null) holdIndicator.fillAmount = 0f;
        }
    }

    public void SetPlayerLooking(bool looking)
    {
        isPlayerLooking = looking;
    }

    IEnumerator FadeInRoutine()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeScreen.color = new Color(0, 0, 0, 0);
    }

    IEnumerator EndShiftRoutine()
    {
        if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlaySFXAtPosition("ExitDoor", transform.position);
        isFading = true;
        if (holdIndicator != null) holdIndicator.fillAmount = 0f;

        float alpha = fadeScreen.color.a;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        bool quotaMet = gameManager.bagsBurnedToday >= gameManager.dailyQuota;
        bool paperworkDone = gameManager.uiCrossedOutCount >= gameManager.dailyQuota && gameManager.uiCheckedCount >= gameManager.dailyQuota;

        if (!quotaMet || !paperworkDone) GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.ShiftAbandonment);

        if (gameManager.fatalRuleBroken)
        {
            GameManager.globalDay = 8;
            Debug.Log("SYSTEM: Loading Day 8 Execution sequence... " + gameManager.ruleBreakReason);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            gameManager.currentDay++;
            GameManager.globalDay = gameManager.currentDay;
            Debug.Log("SYSTEM: Loading Day " + gameManager.currentDay + "...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}