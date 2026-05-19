using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;

    [Header("Tooltip")]
    public GameObject tooltipObject;

    [Header("Ingame UI")]
    public GameObject uiCanvas;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf) OnBackToPause();
            else if (tutorialPanel != null && tutorialPanel.activeSelf) OnBackToPause();
            else TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused) Pause();
        else Resume();
    }

    void Pause()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (uiCanvas != null) uiCanvas.SetActive(false);
        HideTooltip();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerController pc = ServiceLocator.PlayerController;
        if (pc != null)
        {
            pc.canLook = false;
            pc.canMove = false;
        }
    }

    public void Resume()
    {
        // Force-unfocus clipboard if held (prevents canLook/canMove desync)
        PlayerInteraction pi = UnityEngine.Object.FindFirstObjectByType<PlayerInteraction>();
        if (pi != null && pi.heldObject != null)
        {
            ClipboardTool clipboard = pi.heldObject.GetComponent<ClipboardTool>();
            if (clipboard != null && clipboard.isFocused) clipboard.ToggleFocus(false);
        }

        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (uiCanvas != null) uiCanvas.SetActive(true);
        HideTooltip();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerController pc = ServiceLocator.PlayerController;
        if (pc != null)
        {
            pc.canLook = true;
            pc.canMove = true;
        }
    }

    public void OnSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        HideTooltip();
    }

    public void OnTutorial()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        HideTooltip();
    }

    public void OnBackToPause()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
        HideTooltip();
    }

    public void OnQuitToMenu()
    {
        Time.timeScale = 1f;
        GameManager.globalDay = 1;
        GameManager.globalRuleBreakReason = "";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowTooltip()
    {
        if (tooltipObject != null) tooltipObject.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipObject != null) tooltipObject.SetActive(false);
    }
}
