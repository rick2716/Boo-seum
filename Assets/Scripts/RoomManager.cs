using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    [Header("Catálogo")]
    public RoomConfig[] roomCatalog;

    [Header("Spawn")]
    public Transform player;
    public float spawnAheadDistance = 40f; // Genera la próxima sala cuando al jugador le falte esto
    public float spawnBeforeDistance = 40f; // Destruye salas que queden más atrás
    public int prewarm = 2;                // Cuántas salas crear al inicio
    public int noRepeatLastN = 2;          // Evita repetir IDs recientes

    private readonly List<GameObject> activeRooms = new();
    private readonly Queue<string> lastRoomIds = new();
    private int roomsSpawned;
    private int globalSeed;

    [Header("Movimiento de Salas")]
    public float baseRoomSpeed = 5f;         // Velocidad base de las salas
    public float speedIncreaseRate = 0.05f;  // Cuánto aumenta la velocidad por segundo
    public float maxRoomSpeed = 20f;         // Velocidad máxima que alcanzará la sala

    [SerializeField] private float currentSpeed;

    void Start()
    {
        globalSeed = Random.Range(0, int.MaxValue);
        currentSpeed = baseRoomSpeed;  // Asegura que la velocidad inicial no sea cero
        for (int i = 0; i < prewarm; i++) SpawnNextRoom();
    }

    void Update()
    {
        // Incrementa la velocidad de las salas con el tiempo
        IncreaseSpeedOverTime();

        // Mueve todas las salas activas hacia la izquierda
        MoveRooms();

        // ¿Necesitamos una nueva sala?
        if (activeRooms.Count == 0 || NeedNextRoom())
            SpawnNextRoom();

        // Limpia salas que quedaron muy atrás
        DespawnOldRooms();
    }

    private void IncreaseSpeedOverTime()
    {
        // Incrementa la velocidad de las salas, pero no sobrepasa la velocidad máxima
        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.deltaTime, maxRoomSpeed);
    }

    private void MoveRooms()
    {
        // Mueve todas las salas activas hacia la izquierda
        foreach (var room in activeRooms)
        {
            if (room != null)
            {
                room.transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
            }
        }
    }

    private bool NeedNextRoom()
    {
        var last = activeRooms[activeRooms.Count - 1];
        var builder = last.GetComponent<RoomBuilder>();
        float distanceToExit = builder.exitSocket.position.x - player.position.x;
        return distanceToExit < spawnAheadDistance;
    }

    private void SpawnNextRoom()
    {
        var config = PickConfigNonRepeating();
        var instance = Instantiate(config.baseRoomPrefab);

        // Alinear entrada de la nueva sala con la salida de la anterior
        if (activeRooms.Count > 0)
        {
            var prev = activeRooms[activeRooms.Count - 1].GetComponent<RoomBuilder>();
            var newBuilder = instance.GetComponent<RoomBuilder>();

            Vector3 delta = prev.exitSocket.position - newBuilder.entrySocket.position;
            instance.transform.position += delta;
        }

        // Construir variación con semilla distinta
        var rb = instance.GetComponent<RoomBuilder>();
        int seed = globalSeed + roomsSpawned * 7919; // número primo para variar
        rb.Build(config, seed);

        activeRooms.Add(instance);
        roomsSpawned++;

        // Guarda ID para no repetir
        lastRoomIds.Enqueue(config.roomId);
        if (lastRoomIds.Count > noRepeatLastN) lastRoomIds.Dequeue();
    }

    private RoomConfig PickConfigNonRepeating()
    {
        var candidates = roomCatalog
            .Where(c => !lastRoomIds.Contains(c.roomId))
            .ToList();

        if (candidates.Count == 0) candidates = roomCatalog.ToList(); // si no hay, libera restricción

        return candidates[Random.Range(0, candidates.Count)];
    }

    private void DespawnOldRooms()
    {
        // Borra las salas que ya quedaron muy atrás del jugador
        // (Ajusta el umbral según tu cámara)
        float leftLimit = player.position.x - spawnBeforeDistance;

        for (int i = activeRooms.Count - 1; i >= 0; i--)
        {
            var room = activeRooms[i];
            var builder = room.GetComponent<RoomBuilder>();
            if (builder.exitSocket.position.x < leftLimit)
            {
                Destroy(room); // Si usas pooling a nivel de sala, cámbialo por SetActive(false)
                activeRooms.RemoveAt(i);
            }
        }
    }
}
