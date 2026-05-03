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

    void Update()
    {
        if (isFading) return;

        if (isPlayerLooking && Input.GetMouseButton(0))
        {
            currentHoldTime += Time.deltaTime;
            float holdProgress = currentHoldTime / requiredHoldTime;

            if (holdIndicator != null)
            {
                holdIndicator.fillAmount = holdProgress;
            }

            if (currentHoldTime >= requiredHoldTime)
            {
                StartCoroutine(EndShiftRoutine());
            }
        }
        else
        {
            currentHoldTime = 0f;
            if (holdIndicator != null)
            {
                holdIndicator.fillAmount = 0f;
            }
        }
    }

    public void SetPlayerLooking(bool looking)
    {
        isPlayerLooking = looking;
    }

    IEnumerator EndShiftRoutine()
    {
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

        if (!quotaMet || !paperworkDone)
        {
            gameManager.TriggerFutureGameOver("Shift Abandonment: You clocked out prior to completing your daily quota and documentation.");
        }

        if (gameManager.fatalRuleBroken)
        {
            Debug.Log("LOADING EXECUTION SHIFT... " + gameManager.ruleBreakReason);
        }
        else
        {
            GameManager.globalDay++;
            Debug.Log("LOADING DAY " + GameManager.globalDay + "...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}