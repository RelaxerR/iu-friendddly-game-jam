using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    public Transform[] SpawnPoints;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }
}

