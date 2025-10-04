using System.Collections;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class HealthBoostRespawnManager : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef healthBoostPrefab;
    [SerializeField] private float respawnDelay = 20f;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            enabled = false;
            return;
        }

        HealthBoostController.OnBoostConsumed += HandleBoostConsumed;
    }

    private void OnDestroy()
    {
        HealthBoostController.OnBoostConsumed -= HandleBoostConsumed;
    }

    private void HandleBoostConsumed(Vector3 position)
    {
        StartCoroutine(RespawnCoroutine(position));
    }

    private IEnumerator RespawnCoroutine(Vector3 position)
    {
        yield return new WaitForSeconds(respawnDelay);

        Runner.Spawn(
            healthBoostPrefab,
            position,
            Quaternion.identity
        );
    }
}
