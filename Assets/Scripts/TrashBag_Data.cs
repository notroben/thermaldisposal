using UnityEngine;

public class TrashBag_data : MonoBehaviour
{
    public enum WeightCategory { UnderLoad, Optimal, OverCapacity }
    public WeightCategory bagWeight = WeightCategory.Optimal;

    public bool isOrganic = false;

    [Header("Rule Tracking")]
    public bool hasBeenWeighed = false;
    public bool hasBeenRecorded = false;

    [Header("Day 5 Mechanics")]
    public bool isOpen = false;
    public bool isSealed = false;
    public GameObject excessTrashPrefab;

    [Header("Model References")]
    public GameObject closedModel;
    public GameObject openedModel;

    public void OpenBag()
    {
        if (isOpen || isSealed) return;

        isOpen = true;

        if (closedModel != null) closedModel.SetActive(false);
        if (openedModel != null) openedModel.SetActive(true);

        if (excessTrashPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject trash = Instantiate(excessTrashPrefab, spawnPos, Random.rotation);

            Rigidbody rb = trash.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(Vector3.up * 3f + Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }
    }

    public void SealBag()
    {
        if (!isOpen || isSealed) return;

        isOpen = false;
        isSealed = true;
        bagWeight = WeightCategory.Optimal;

        if (openedModel != null) openedModel.SetActive(false);
        if (closedModel != null) closedModel.SetActive(true);
    }
}