using System;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab; // Префаб модели игрока
    public Transform[] SpawnPoints; // Массив заранее расставленных спавнпоинтов

    public void PlayerJoined(PlayerRef player)
    {
        try
        {
            Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            Runner.Spawn(PlayerPrefab, spawnPoint.position, spawnPoint.rotation, player);
        }
        catch (Exception e)
        {
            Debug.LogError($"Не удалось создать объект игрока: {e.Message}\n{e.StackTrace}");
            throw;
        }
    }
}