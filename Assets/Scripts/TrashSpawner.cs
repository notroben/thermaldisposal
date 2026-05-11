using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject trashBagPrefab;
    public GameObject organicBagPrefab;
    public GameObject emptyBagPrefab;
    public Transform emptyBagSpawnPoint;

    [Header("Spawning Rules")]
    public int maxBags = 10;
    public float minSpawnTime = 10f;
    public float maxSpawnTime = 30f;

    private float timer;
    private float currentSpawnInterval;
    private int bagsSpawned = 0;
    private GameManager gameManager;

    private List<int> day4OrganicIndices = new List<int>();
    private List<int> day5OverCapacityIndices = new List<int>();

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            if (gameManager.currentDay == 4) GenerateRandomIndices(day4OrganicIndices, 3);
            if (gameManager.currentDay == 5)
            {
                GenerateRandomIndices(day5OverCapacityIndices, 5);

                if (emptyBagPrefab != null && emptyBagSpawnPoint != null)
                {
                    Instantiate(emptyBagPrefab, emptyBagSpawnPoint.position, Quaternion.identity);
                }
            }
        }

        SetRandomInterval();
    }

    void GenerateRandomIndices(List<int> listToFill, int amount)
    {
        while (listToFill.Count < amount)
        {
            int randomIndex = Random.Range(0, maxBags);
            if (!listToFill.Contains(randomIndex)) listToFill.Add(randomIndex);
        }
    }

    void Update()
    {
        if (gameManager != null && gameManager.currentDay >= 7) return;
        if (bagsSpawned < maxBags)
        {
            timer += Time.deltaTime;

            if (timer >= currentSpawnInterval)
            {
                SpawnBag();
                bagsSpawned++;
                timer = 0f;
                SetRandomInterval();
            }
        }
    }

    void SpawnBag()
    {
        bool spawnOrganic = false;
        bool spawnHeavy = false;

        if (gameManager != null)
        {
            if (gameManager.currentDay == 2 && bagsSpawned == 8) spawnOrganic = true;
            else if (gameManager.currentDay == 4 && day4OrganicIndices.Contains(bagsSpawned)) spawnOrganic = true;
            else if (gameManager.currentDay == 5 && day5OverCapacityIndices.Contains(bagsSpawned)) spawnHeavy = true;
        }

        GameObject newBag;
        if (spawnOrganic) newBag = Instantiate(organicBagPrefab, transform.position, Quaternion.identity);
        else newBag = Instantiate(trashBagPrefab, transform.position, Quaternion.identity);

        if (spawnHeavy)
        {
            TrashBag_data data = newBag.GetComponent<TrashBag_data>();
            if (data != null) data.bagWeight = TrashBag_data.WeightCategory.OverCapacity;
        }
    }

    void SetRandomInterval()
    {
        currentSpawnInterval = Random.Range(minSpawnTime, maxSpawnTime);
    }
}