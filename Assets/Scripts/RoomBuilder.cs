using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    public Transform entrySocket;
    public Transform exitSocket;

    private System.Random prng;

    public void Build(RoomConfig config, int seed)
    {
        prng = new System.Random(seed);

        var markers = GetComponentsInChildren<SpawnMarker>(true);
        foreach (var m in markers)
        {
            // Probabilidad de omitir para que no todo se vea "lleno"
            if (Random.value < config.skipProbability) continue;

            GameObject toSpawn = null;
            switch (m.type)
            {
                case MarkerType.Floor:
                    toSpawn = WeightedPick(config.floorObstacles);
                    break;
                case MarkerType.Ceiling:
                    toSpawn = WeightedPick(config.ceilingObstacles);
                    break;
                case MarkerType.Decor:
                    toSpawn = WeightedPick(config.decor);
                    break;
            }

            if (toSpawn != null)
            {
                // Si usas pooling, aquí llamarías a PoolManager.Get(toSpawn, pos)
                Instantiate(toSpawn, m.transform.position, m.transform.rotation, transform);
            }
        }
    }

    private GameObject WeightedPick(RoomConfig.SpawnEntry[] entries)
    {
        if (entries == null || entries.Length == 0) return null;

        float total = 0f;
        foreach (var e in entries) total += Mathf.Max(0, e.weight);
        float r = Random.value * total;

        foreach (var e in entries)
        {
            r -= Mathf.Max(0, e.weight);
            if (r <= 0f) return e.prefab;
        }
        return entries[entries.Length - 1].prefab; // fallback
    }
}
