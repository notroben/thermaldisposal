using UnityEngine;
using System.Collections.Generic;

public class FurnaceLogic : MonoBehaviour
{
    [Header("Connections")]
    public GameManager gameManager;

    [Header("Furnace State")]
    public bool isDoorClosed = false;

    private List<GameObject> itemsInside = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrashBag_data>() != null || other.CompareTag("Player") || other.CompareTag("Tool"))
        {
            if (!itemsInside.Contains(other.gameObject))
            {
                itemsInside.Add(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (itemsInside.Contains(other.gameObject))
        {
            itemsInside.Remove(other.gameObject);
        }
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
        foreach (GameObject item in itemsInside)
        {
            if (item.GetComponent<TrashBag_data>() != null) bagsToBurn++;
        }

        if (bagsToBurn > 0)
        {
            gameManager.VerifyFurnaceHonesty(bagsToBurn);
        }

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
            else if (item.GetComponent<TrashBag_data>() != null)
            {
                TrashBag_data bagData = item.GetComponent<TrashBag_data>();

                if (bagData.bagWeight == TrashBag_data.WeightCategory.OverCapacity)
                {
                    gameManager.TriggerFutureGameOver("Protocol Violation: Incinerated OVER_CAPACITY material. Excess debris permanently jammed the furnace.");

                    if (bagData.excessTrashPrefab != null) Instantiate(bagData.excessTrashPrefab, item.transform.position, Quaternion.identity);
                }

                if (bagData.isOrganic) Debug.Log("AUDIO: Human-like scream audio plays");

                gameManager.AddBurnedBag(bagData);
                Destroy(item);
            }
        }
        itemsInside.Clear();
    }

    void TriggerEndingSequence()
    {
        Debug.Log("THERMAL DISPOSAL: INITIATED. GAME OVER."); // basically end screen, will be polished later
    }
}