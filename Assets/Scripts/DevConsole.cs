using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class DevConsole : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    [Header("UI References")]
    public GameObject consolePanel;
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;
    public ScrollRect scrollRect;

    [Header("HUD Reference")]
    public GameObject hudCanvas;

    [Header("Spawnable Prefabs")]
    public GameObject[] spawnablePrefabs;

    private bool cheatsEnabled = false;
    private bool noclipActive = false;
    private bool godModeActive = false;
    private List<string> logLines = new List<string>();
    private float previousTimeScale = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (IsOpen) CloseConsole();
            else OpenConsole();
        }

        if (!IsOpen) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            string command = inputField.text.Trim();
            if (!string.IsNullOrEmpty(command))
            {
                ProcessCommand(command);
                inputField.text = "";
            }
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    void OpenConsole()
    {
        IsOpen = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (consolePanel != null) consolePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerController pc = ServiceLocator.PlayerController;
        if (pc != null)
        {
            pc.canLook = false;
            pc.canMove = false;
        }

        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    void CloseConsole()
    {
        IsOpen = false;
        Time.timeScale = previousTimeScale;

        if (consolePanel != null) consolePanel.SetActive(false);

        PlayerController pc = ServiceLocator.PlayerController;
        if (pc != null)
        {
            pc.canLook = true;
            pc.canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void ProcessCommand(string rawCommand)
    {
        Log($"> {rawCommand}");
        string cmd = rawCommand.ToLower().Trim();

        if (cmd == "imthefuckingdeveloper")
        {
            cheatsEnabled = !cheatsEnabled;
            Log(cheatsEnabled ? "CHEATS ENABLED." : "CHEATS DISABLED.");
            return;
        }

        if (cmd == "help")
        {
            Log("--- COMMANDS ---");
            Log("  togglehud   -  Toggle crosshair and prompt text");
            Log("  help        -  Show this list");
            Log("  clear       -  Clear console output");
            if (cheatsEnabled)
            {
                Log("--- CHEAT COMMANDS ---");
                Log("  startday X       -  Jump to day X (1-8)");
                Log("  spawnobject X    -  Spawn a prefab at look position");
                Log("  god              -  Toggle god mode (ignore infractions)");
                Log("  noclip           -  Toggle noclip (fly through walls)");
                Log("  listobjects      -  List spawnable object names");
            }
            return;
        }

        if (cmd == "clear")
        {
            logLines.Clear();
            if (outputText != null) outputText.text = "";
            return;
        }

        if (cmd == "togglehud")
        {
            if (hudCanvas != null)
            {
                hudCanvas.SetActive(!hudCanvas.activeSelf);
                Log($"HUD {(hudCanvas.activeSelf ? "SHOWN" : "HIDDEN")}");
            }
            else
            {
                Log("ERROR: There is no HUD.");
            }
            return;
        }

        if (!cheatsEnabled)
        {
            Log($"Unknown command: {rawCommand}");
            return;
        }

        if (cmd.StartsWith("startday "))
        {
            string dayStr = cmd.Substring("startday ".Length).Trim();
            if (int.TryParse(dayStr, out int day) && day >= 1 && day <= 8)
            {
                Log($"Starting Day {day}...");
                GameManager.globalDay = day;
                if (day == 8 && string.IsNullOrEmpty(GameManager.globalRuleBreakReason)) GameManager.globalRuleBreakReason = "CHEAT: Forced execution day.";
                IsOpen = false;
                Time.timeScale = 1f;
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                Log("ERROR: Invalid day number. Use 1-8.");
            }
            return;
        }

        if (cmd.StartsWith("spawnobject "))
        {
            string objName = cmd.Substring("spawnobject ".Length).Trim();
            SpawnObject(objName);
            return;
        }

        if (cmd == "listobjects")
        {
            Log("--- SPAWNABLE OBJECTS ---");
            if (spawnablePrefabs != null)
            {
                foreach (GameObject prefab in spawnablePrefabs)
                {
                    if (prefab != null) Log($"  {prefab.name.ToLower()}");
                }
            }
            return;
        }

        if (cmd == "god")
        {
            godModeActive = !godModeActive;
            GameManager.godMode = godModeActive;
            Log(godModeActive
                ? "GOD MODE ON - Infractions will be ignored."
                : "GOD MODE OFF");
            return;
        }

        if (cmd == "noclip")
        {
            PlayerController pc = ServiceLocator.PlayerController;
            if (pc == null)
            {
                Log("ERROR: No player found.");
                return;
            }

            noclipActive = !noclipActive;
            pc.noclipEnabled = noclipActive;

            CharacterController cc = pc.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = !noclipActive;

            Log(noclipActive
                ? "NOCLIP ON - Use Space/Shift for vertical movement."
                : "NOCLIP OFF");
            return;
        }
        Log($"Unknown command: {rawCommand}");
    }

    void SpawnObject(string objectName)
    {
        if (spawnablePrefabs == null || spawnablePrefabs.Length == 0)
        {
            Log("ERROR: No spawnable prefabs configured.");
            return;
        }

        GameObject prefab = null;
        foreach (GameObject p in spawnablePrefabs)
        {
            if (p != null && p.name.ToLower() == objectName)
            {
                prefab = p;
                break;
            }
        }

        if (prefab == null)
        {
            Log($"ERROR: Unknown object '{objectName}'. Type 'list objects' for valid names.");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Log("ERROR: No object spawning in Main Menu.");
            return;
        }

        Vector3 spawnPos;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 5f))
        {
            spawnPos = hit.point + Vector3.up * 0.5f;
        }
        else
        {
            spawnPos = cam.transform.position + cam.transform.forward * 3f;
        }

        Instantiate(prefab, spawnPos, Quaternion.identity);
        Log($"Spawned '{prefab.name}' at {spawnPos}");
    }

    void Log(string message)
    {
        logLines.Add(message);

        if (logLines.Count > 50) logLines.RemoveAt(0);

        if (outputText != null) outputText.text = string.Join("\n", logLines);

        if (scrollRect != null && scrollRect.content != null)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
