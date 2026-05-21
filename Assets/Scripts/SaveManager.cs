using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int slotIndex;
    public int currentDay;
    public string ruleBreakReason;
    public string saveTimestamp;
}

public static class SaveManager
{
    public static Texture2D CachedScreenshot;

    private static string SaveDirectory => Path.Combine(Application.persistentDataPath, "saves");

    private static string GetJsonPath(int slot) => Path.Combine(SaveDirectory, $"save_slot_{slot}.json");
    private static string GetScreenshotPath(int slot) => Path.Combine(SaveDirectory, $"save_slot_{slot}.png");

    public static void SaveGame(int slotIndex)
    {
        if (!Directory.Exists(SaveDirectory)) Directory.CreateDirectory(SaveDirectory);

        SaveData data = new SaveData
        {
            slotIndex = slotIndex,
            currentDay = GameManager.globalDay,
            ruleBreakReason = GameManager.globalRuleBreakReason,
            saveTimestamp = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm")
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetJsonPath(slotIndex), json);

        if (CachedScreenshot != null)
        {
            byte[] pngBytes = CachedScreenshot.EncodeToPNG();
            File.WriteAllBytes(GetScreenshotPath(slotIndex), pngBytes);
        }

        Debug.Log($"SYSTEM: Game saved to slot {slotIndex} (Day {data.currentDay})");
    }

    public static void LoadGame(int slotIndex)
    {
        SaveData data = GetSaveData(slotIndex);
        if (data == null) return;

        GameManager.globalDay = data.currentDay;
        GameManager.globalRuleBreakReason = data.ruleBreakReason ?? "";

        Debug.Log($"SYSTEM: Loading save slot {slotIndex} (Day {data.currentDay})");
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    public static void DeleteSave(int slotIndex)
    {
        string jsonPath = GetJsonPath(slotIndex);
        string screenshotPath = GetScreenshotPath(slotIndex);

        if (File.Exists(jsonPath)) File.Delete(jsonPath);
        if (File.Exists(screenshotPath)) File.Delete(screenshotPath);

        Debug.Log($"SYSTEM: Save slot {slotIndex} deleted.");
    }

    public static bool HasSave(int slotIndex)
    {
        return File.Exists(GetJsonPath(slotIndex));
    }

    public static SaveData GetSaveData(int slotIndex)
    {
        string path = GetJsonPath(slotIndex);
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static Texture2D LoadScreenshot(int slotIndex)
    {
        string path = GetScreenshotPath(slotIndex);
        if (!File.Exists(path)) return null;

        byte[] bytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        return tex;
    }

    public static Texture2D ResizeScreenshot(Texture2D source, int targetWidth, int targetHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }
}
