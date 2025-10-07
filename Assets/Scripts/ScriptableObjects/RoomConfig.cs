using UnityEngine;

[CreateAssetMenu(menuName = "Runner/Room Config")]
public class RoomConfig : ScriptableObject
{
    [Header("Identidad")]
    public string roomId;                    // p.ej. "Galeria_A"
    public GameObject baseRoomPrefab;        // Prefab de sala con sockets y marcadores

    [Header("Variación y densidad")]
    [Range(0,1)] public float skipProbability = 0.2f; // prob. de omitir un marcador
    public AnimationCurve obstacleDensityByX = AnimationCurve.Linear(0,0.4f,1,0.7f);

    [Header("Spawn Tables (pesos)")]
    public SpawnEntry[] floorObstacles;
    public SpawnEntry[] ceilingObstacles;
    public SpawnEntry[] decor;

    [System.Serializable]
    public struct SpawnEntry
    {
        public GameObject prefab;
        public float weight; // probabilidad relativa
    }
}
