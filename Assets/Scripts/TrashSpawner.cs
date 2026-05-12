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

    private List<int> randomOrganicIndices = new List<int>();
    private List<int> randomOverCapacityIndices = new List<int>();

    void Start()
    {
        gameManager = ServiceLocator.GameManager;

        if (gameManager != null && gameManager.CurrentDayData != null)
        {
            DayData config = gameManager.CurrentDayData;
            
            if (config.randomOrganicCount > 0) 
                GenerateRandomIndices(randomOrganicIndices, config.randomOrganicCount);
                
            if (config.randomOverCapacityCount > 0)
                GenerateRandomIndices(randomOverCapacityIndices, config.randomOverCapacityCount);

            if (config.spawnEmptyBag && emptyBagPrefab != null && emptyBagSpawnPoint != null)
            {
                Instantiate(emptyBagPrefab, emptyBagSpawnPoint.position, Quaternion.identity);
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
        if (gameManager != null && gameManager.CurrentDayData != null && (gameManager.CurrentDayData.isFinalDay || gameManager.CurrentDayData.isExecutionDay)) return;
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

        if (gameManager != null && gameManager.CurrentDayData != null)
        {
            DayData config = gameManager.CurrentDayData;
            if (config.specificOrganicIndex == bagsSpawned) spawnOrganic = true;
            else if (randomOrganicIndices.Contains(bagsSpawned)) spawnOrganic = true;
            
            if (randomOverCapacityIndices.Contains(bagsSpawned)) spawnHeavy = true;
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