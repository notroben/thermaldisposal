using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Camera Parallax")]
    public Transform menuCamera;
    public float parallaxAmount = 1.5f;
    public float parallaxSmoothing = 2f;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;
    public SaveLoadPanel saveLoadPanel;

    private Quaternion cameraStartRot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        if (menuCamera != null) cameraStartRot = menuCamera.rotation;

        ShowMainPanel();
    }

    void Update()
    {
        if (menuCamera != null)
        {
            float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
            float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

            Quaternion targetRot = cameraStartRot * Quaternion.Euler(
                -mouseY * parallaxAmount,
                mouseX * parallaxAmount,
                0f
            );

            menuCamera.rotation = Quaternion.Slerp(menuCamera.rotation, targetRot, Time.deltaTime * parallaxSmoothing);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (DevConsole.IsOpen) return;

            if (settingsPanel != null && settingsPanel.activeSelf) OnBackToMain();
            else if (tutorialPanel != null && tutorialPanel.activeSelf) OnBackToMain();
            else if (saveLoadPanel != null && saveLoadPanel.gameObject.activeSelf) OnBackToMain();
        }
    }

    public void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.ClosePanel();
    }

    public void OnNewGame()
    {
        GameManager.globalDay = 1;
        GameManager.globalRuleBreakReason = "";
        SceneManager.LoadScene("SampleScene");
    }

    public void OnLoadGame()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.OpenPanel(SaveLoadPanel.Mode.Load);
    }

    public void OnTutorial()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
    }

    public void OnSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OnBackToMain()
    {
        ShowMainPanel();
    }

    public void OnQuit()
    {
        Debug.Log("SYSTEM: Quitting application.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
