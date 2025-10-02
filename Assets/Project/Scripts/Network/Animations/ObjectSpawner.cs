using Fusion;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef animatedPrefab;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Spawn(animatedPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
