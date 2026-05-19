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
        if (menuCamera == null) return;

        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        Quaternion targetRot = cameraStartRot * Quaternion.Euler(
            -mouseY * parallaxAmount,
            mouseX * parallaxAmount,
            0f
        );

        menuCamera.rotation = Quaternion.Slerp(menuCamera.rotation, targetRot, Time.deltaTime * parallaxSmoothing);
    }

    public void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
    }

    public void OnNewGame()
    {
        GameManager.globalDay = 1;
        GameManager.globalRuleBreakReason = "";
        SceneManager.LoadScene("SampleScene");
    }

    public void OnLoadGame()
    {
        // placeholder
        Debug.Log("SYSTEM: Load Game not yet implemented.");
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
