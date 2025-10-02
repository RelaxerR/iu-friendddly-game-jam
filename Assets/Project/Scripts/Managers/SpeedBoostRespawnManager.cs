using System.Collections;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class SpeedBoostRespawnManager : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef speedBoostPrefab;
    [SerializeField] private float respawnDelay = 20f;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            enabled = false;
            return;
        }

        SpeedBoostController.OnBoostConsumed += HandleBoostConsumed;
    }

    private void OnDestroy()
    {
        SpeedBoostController.OnBoostConsumed -= HandleBoostConsumed;
    }

    private void HandleBoostConsumed(Vector3 position, Quaternion rotation)
    {
        StartCoroutine(RespawnCoroutine(position, rotation));
    }

    private IEnumerator RespawnCoroutine(Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(respawnDelay);

        Runner.Spawn(
            speedBoostPrefab,
            position,
            rotation
        );
    }
}
