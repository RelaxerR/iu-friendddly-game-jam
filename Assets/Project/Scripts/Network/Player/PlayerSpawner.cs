using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab; // Префаб модели игрока
    public Transform[] SpawnPoints; // Массив заранее расставленных спавнпоинтов

    public void PlayerJoined(PlayerRef player)
    {
        Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Runner.Spawn(PlayerPrefab, spawnPoint.position, spawnPoint.rotation, player);
    }
}