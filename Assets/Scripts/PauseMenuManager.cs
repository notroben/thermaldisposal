using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;
    public SaveLoadPanel saveLoadPanel;

    [Header("Tooltip")]
    public GameObject tooltipObject;

    [Header("Ingame UI")]
    public GameObject uiCanvas;

    private bool isPaused = false;
    private bool isCapturingScreenshot = false;

    void Update()
    {
        if (DevConsole.IsOpen) return;
        if (isCapturingScreenshot) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf) OnBackToPause();
            else if (tutorialPanel != null && tutorialPanel.activeSelf) OnBackToPause();
            else if (saveLoadPanel != null && saveLoadPanel.gameObject.activeSelf) OnBackToPause();
            else TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused) StartCoroutine(PauseWithScreenshot());
        else Resume();
    }

    IEnumerator PauseWithScreenshot()
    {
        isCapturingScreenshot = true;
        isPaused = true;

        yield return new WaitForEndOfFrame();

        Texture2D fullScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        fullScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        fullScreenshot.Apply();
        if (SaveManager.CachedScreenshot != null) Object.Destroy(SaveManager.CachedScreenshot);
        SaveManager.CachedScreenshot = SaveManager.ResizeScreenshot(fullScreenshot, 320, 180);
        Object.Destroy(fullScreenshot);

        isCapturingScreenshot = false;

        DoPause();
    }

    void DoPause()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.ClosePanel();
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
        PlayerInteraction pi = Object.FindFirstObjectByType<PlayerInteraction>();
        if (pi != null && pi.heldObject != null)
        {
            ClipboardTool clipboard = pi.heldObject.GetComponent<ClipboardTool>();
            if (clipboard != null && clipboard.isFocused) clipboard.ToggleFocus(false);
        }

        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.ClosePanel();
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

    public void OnSaveGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.OpenPanel(SaveLoadPanel.Mode.Save);
        HideTooltip();
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
        if (saveLoadPanel != null) saveLoadPanel.ClosePanel();
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
