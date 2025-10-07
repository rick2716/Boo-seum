using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnPoint
    {
        public Transform spawnTransform;
        public GameObject[] prefabs;
    }

    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float increasingSpawnInterval;
    public float spawnInterval = 2f;
    [Range(0, 1)] public float spawnIntervalFactor = 0.1f;
    [SerializeField] private float increasingObjectSpeed;
    public float objectSpeed = 5f;
    [Range(0, 1)] public float objectSpeedFactor = 0.2f;

    private List<GameObject> pool = new List<GameObject>();
    private float timeAlive;
    private float timer;

    void Start()
    {
        ResetFactors();
        for (int i = 0; i < poolSize; i++)
        {
            SpawnPoint randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = randomPoint.prefabs[Random.Range(0, randomPoint.prefabs.Length)];

            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        timeAlive += Time.deltaTime;

        if (timer >= increasingSpawnInterval)
        {
            CalculateIncreasingSpeed();
            SpawnFromPool();
            timer = 0f;
        }
    }

    private void CalculateIncreasingSpeed()
    {
        increasingObjectSpeed = objectSpeed * Mathf.Pow(timeAlive, objectSpeedFactor);
        increasingSpawnInterval = spawnInterval / Mathf.Pow(timeAlive, spawnIntervalFactor);
    }

    private void ResetFactors()
    {
        timeAlive = 1f;
        increasingObjectSpeed = objectSpeed;
        increasingSpawnInterval = spawnInterval;
    }

    private void SpawnFromPool()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                SpawnPoint selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                obj.transform.position = selectedPoint.spawnTransform.position;
                obj.SetActive(true);

                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                rb.linearVelocity = Vector2.left * increasingObjectSpeed;
                break;
            }
        }
    }
}