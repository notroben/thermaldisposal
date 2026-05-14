using UnityEngine;

[CreateAssetMenu(fileName = "New Day Data", menuName = "Thermal Disposal/Day Data")]
public class DayData : ScriptableObject
{
    [Header("General Settings")]
    public int dailyQuota = 10;
    [TextArea(3, 10)]
    public string directiveText = "DAY X DIRECTIVE:\n...";

    [Header("Facility Systems")]
    public bool conveyorBroken = false;
    public bool unlockScanner = false;
    public bool unlockIronPoker = false;
    public bool showTaskList = true;
    public bool showExitDoor = true;
    public bool showDay5ExtraTask = false;

    [Header("Spawning Rules")]
    public int randomOrganicCount = 0;
    public int specificOrganicIndex = -1;
    public int randomOverCapacityCount = 0;
    public bool spawnEmptyBag = false;
    
    [Header("Hazards & Special Events")]
    public bool hasCharredDebris = false;
    public bool isFinalDay = false;
    public bool isExecutionDay = false;
}
