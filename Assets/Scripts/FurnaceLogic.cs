using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FurnaceLogic : MonoBehaviour
{
    [Header("Connections")]

    [Header("Furnace State")]
    public bool isDoorClosed = false;
    private bool endingTriggered = false;
    [HideInInspector] public bool isProcessing = false;

    [Header("Day 6 Incomplete Combustion")]
    public GameObject[] day6CharredDebrisPrefabs;

    private List<GameObject> itemsInside = new List<GameObject>();

    void OnEnable()
    {
        GameEvents.OnFurnaceActivated += ActivateFurnace;
        GameEvents.OnFurnaceDoorToggled += SetDoorState;
    }

    void OnDisable()
    {
        GameEvents.OnFurnaceActivated -= ActivateFurnace;
        GameEvents.OnFurnaceDoorToggled -= SetDoorState;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrashBag_data>() != null || other.CompareTag("Player") || other.CompareTag("Tool") || other.GetComponent<DebrisLogic>() != null || other.CompareTag("ExcessTrash"))
        {
            if (!itemsInside.Contains(other.gameObject)) itemsInside.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (itemsInside.Contains(other.gameObject)) itemsInside.Remove(other.gameObject);
    }

    public void ActivateFurnace()
    {
        if (isProcessing) return;

        if (!isDoorClosed)
        {
            Debug.Log("SYSTEM WARNING: Thermal Disposal unit cannot activate while safety door is ajar.");
            // if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlaySFXAtPosition("ErrorBeep", transform.position);
            return;
        }

        StartCoroutine(ProcessingRoutine());
    }

    IEnumerator ProcessingRoutine()
    {
        isProcessing = true;

        Debug.Log("SYSTEM: Thermal Disposal unit activated. Processing...");
        if (ServiceLocator.AudioManager != null)
        {
            ServiceLocator.AudioManager.PlaySFXAtPosition("FurnaceSwitch", transform.position);
            ServiceLocator.AudioManager.PlaySFXAtPosition("FurnaceRoar", transform.position);
        } 

        itemsInside.RemoveAll(item => item == null || !item.activeInHierarchy || (item.GetComponent<Collider>() != null && !item.GetComponent<Collider>().enabled));

        int bagsToBurn = 0;
        int charredDebrisCount = 0;

        foreach (GameObject item in itemsInside)
        {
            if (item.GetComponent<TrashBag_data>() != null) bagsToBurn++;
            DebrisLogic debris = item.GetComponent<DebrisLogic>();
            if (debris != null && debris.isCharred) charredDebrisCount++;
        }

        if (bagsToBurn > 0 && charredDebrisCount > 0) GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.FurnaceJammedDebris);

        if (bagsToBurn > 0 && ServiceLocator.GameManager != null) ServiceLocator.GameManager.VerifyFurnaceHonesty(bagsToBurn);

        List<GameObject> pendingDebris = new List<GameObject>();

        for (int i = itemsInside.Count - 1; i >= 0; i--)
        {
            GameObject item = itemsInside[i];

            if (item.CompareTag("Player") && ServiceLocator.GameManager.CurrentDayData.isFinalDay)
            {
                TriggerEndingSequence();
            }
            else if (item.CompareTag("Tool"))
            {
                GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.PropertyDestruction);
                Destroy(item);
            }
            else if (item.GetComponent<DebrisLogic>() != null)
            {
                item.GetComponent<DebrisLogic>().CharDebris();
            }
            else if (item.GetComponent<TrashBag_data>() != null)
            {
                TrashBag_data bagData = item.GetComponent<TrashBag_data>();

                if (bagData.bagWeight == TrashBag_data.WeightCategory.OverCapacity)
                {
                    GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.FurnaceJammedOverCapacity);

                    if (bagData.excessTrashPrefab != null)
                    {
                        GameObject debris = Instantiate(bagData.excessTrashPrefab, item.transform.position, Quaternion.identity);
                        if (debris.GetComponent<DebrisLogic>() != null) debris.GetComponent<DebrisLogic>().CharDebris();
                        pendingDebris.Add(debris);
                    }
                }

                if (bagData.isOrganic)
                {
                    Debug.Log("AUDIO TRIGGER: Play organic matter scream SFX");
                    if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlaySFXAtPosition("OrganicScream", transform.position);
                }

                if (ServiceLocator.GameManager != null) ServiceLocator.GameManager.AddBurnedBag(bagData);

                if (ServiceLocator.GameManager != null && ServiceLocator.GameManager.CurrentDayData != null && ServiceLocator.GameManager.CurrentDayData.hasCharredDebris && day6CharredDebrisPrefabs.Length > 0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        GameObject randomDebris = day6CharredDebrisPrefabs[Random.Range(0, day6CharredDebrisPrefabs.Length)];
                        Vector3 spawnOffset = new Vector3(Random.Range(-0.2f, 0.2f), j * 0.5f, Random.Range(-0.2f, 0.2f));

                        GameObject spawnedObj = Instantiate(randomDebris, item.transform.position + spawnOffset, Random.rotation);

                        DebrisLogic debrisLogic = spawnedObj.GetComponent<DebrisLogic>();
                        if (debrisLogic != null) debrisLogic.CharDebris();

                        pendingDebris.Add(spawnedObj);
                    }
                }

                Destroy(item);
            }
        }

        itemsInside.AddRange(pendingDebris);

        yield return new WaitForSeconds(5f);
        isProcessing = false;
    }

    public void SetDoorState(bool closedState)
    {
        isDoorClosed = closedState;
        if (ServiceLocator.AudioManager != null)
        {
            ServiceLocator.AudioManager.PlaySFXAtPosition(isDoorClosed ? "FurnaceDoorClose" : "FurnaceDoorOpen", transform.position);
        }

        if (ServiceLocator.GameManager != null && ServiceLocator.GameManager.CurrentDayData != null && ServiceLocator.GameManager.CurrentDayData.isFinalDay && isDoorClosed && !endingTriggered)
        {
            foreach (GameObject item in itemsInside)
            {
                if (item != null && item.CompareTag("Player"))
                {
                    endingTriggered = true;
                    TriggerEndingSequence();
                    break;
                }
            }
        }
    }

    void TriggerEndingSequence()
    {
        Debug.Log("SYSTEM: Final Protocol Initiated. Commencing Employee Execution.");

        if (ServiceLocator.GameManager != null)
        {
            ServiceLocator.GameManager.TriggerTrueEnding();
        }
    }
}