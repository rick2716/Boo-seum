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
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float objectSpeed = 5f;

    private List<GameObject> pool = new List<GameObject>();
    private float timer;

    void Start()
    {
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

        if (timer >= spawnInterval)
        {
            SpawnFromPool();
            timer = 0f;
        }
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
                rb.linearVelocity = Vector2.left * objectSpeed;
                break;
            }
        }
    }
}