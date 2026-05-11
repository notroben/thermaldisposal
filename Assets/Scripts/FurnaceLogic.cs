using UnityEngine;
using System.Collections.Generic;

public class FurnaceLogic : MonoBehaviour
{
    [Header("Connections")]
    public GameManager gameManager;

    [Header("Furnace State")]
    public bool isDoorClosed = false;
    private bool endingTriggered = false;

    [Header("Day 6 Incomplete Combustion")]
    public GameObject[] day6CharredDebrisPrefabs;

    private List<GameObject> itemsInside = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrashBag_data>() != null || other.CompareTag("Player") || other.CompareTag("Tool") || other.GetComponent<DebrisLogic>() != null)
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
        if (!isDoorClosed)
        {
            Debug.Log("ERROR: Cannot start! Furnace door is open.");
            return;
        }

        Debug.Log("FURNACE ACTIVATED! Incinerating contents...");

        itemsInside.RemoveAll(item => item == null || !item.activeInHierarchy || (item.GetComponent<Collider>() != null && !item.GetComponent<Collider>().enabled));

        int bagsToBurn = 0;
        int charredDebrisCount = 0;

        foreach (GameObject item in itemsInside)
        {
            if (item.GetComponent<TrashBag_data>() != null) bagsToBurn++;
            DebrisLogic debris = item.GetComponent<DebrisLogic>();
            if (debris != null && debris.isCharred) charredDebrisCount++;
        }

        if (bagsToBurn > 0 && charredDebrisCount > 0) gameManager.TriggerFutureGameOver("Equipment Neglect: Attempted to process new material while furnace was obstructed by uncleared carbonized debris.");

        if (bagsToBurn > 0) gameManager.VerifyFurnaceHonesty(bagsToBurn);

        for (int i = itemsInside.Count - 1; i >= 0; i--)
        {
            GameObject item = itemsInside[i];

            if (item.CompareTag("Player"))
            {
                TriggerEndingSequence();
            }
            else if (item.CompareTag("Tool"))
            {
                gameManager.TriggerFutureGameOver("Destruction of essential company property.");
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
                    gameManager.TriggerFutureGameOver("Protocol Violation: Incinerated OVER_CAPACITY material. Excess debris permanently jammed the furnace.");

                    if (bagData.excessTrashPrefab != null)
                    {
                        GameObject debris = Instantiate(bagData.excessTrashPrefab, item.transform.position, Quaternion.identity);
                        if (debris.GetComponent<DebrisLogic>() != null) debris.GetComponent<DebrisLogic>().CharDebris();
                        itemsInside.Add(debris);
                    }
                }

                if (bagData.isOrganic) Debug.Log("AUDIO: Human-like scream audio plays");

                gameManager.AddBurnedBag(bagData);

                if (gameManager.currentDay == 6 && day6CharredDebrisPrefabs.Length > 0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        GameObject randomDebris = day6CharredDebrisPrefabs[Random.Range(0, day6CharredDebrisPrefabs.Length)];
                        Vector3 spawnOffset = new Vector3(Random.Range(-0.2f, 0.2f), j * 0.5f, Random.Range(-0.2f, 0.2f));

                        GameObject spawnedObj = Instantiate(randomDebris, item.transform.position + spawnOffset, Random.rotation);

                        DebrisLogic debrisLogic = spawnedObj.GetComponent<DebrisLogic>();
                        if (debrisLogic != null) debrisLogic.CharDebris();

                        itemsInside.Add(spawnedObj);
                    }
                }

                Destroy(item);
            }
        }
    }

    public void SetDoorState(bool closedState)
    {
        isDoorClosed = closedState;

        if (gameManager.currentDay >= 7 && isDoorClosed && !endingTriggered)
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
        Debug.Log("THERMAL DISPOSAL: INITIATED. GAME OVER.");

        if (gameManager != null)
        {
            gameManager.TriggerTrueEnding();
        }
    }
}