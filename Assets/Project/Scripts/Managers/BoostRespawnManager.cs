using System.Collections;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class BoostRespawnManager : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NetworkPrefabRef healthBoostPrefab; // Ссылка на префаб с бустером HP (оранжевая баночка мёда)
    [SerializeField] private NetworkPrefabRef speedBoostPrefab; // Ссылка на префаб с бустером скорости (фиолетовая баночка мёда)

    [Header("Respawn")]
    [SerializeField] private float healthRespawnDelay = 17f; // Длительность респавна бустера HP
    [SerializeField] private float speedRespawnDelay = 22f; // Длительность респавна бустера скорости

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            enabled = false;
            return;
        }

        HealthBoostController.OnBoostConsumed += HandleHealthBoostConsumed;
        SpeedBoostController.OnBoostConsumed += HandleSpeedBoostConsumed;
    }

    private void OnDestroy()
    {
        HealthBoostController.OnBoostConsumed -= HandleHealthBoostConsumed;
        SpeedBoostController.OnBoostConsumed -= HandleSpeedBoostConsumed;
    }

    private void HandleHealthBoostConsumed(NetworkObject parentNetObj, Vector3 localPos, Quaternion localRot)
    {
        StartCoroutine(RespawnCoroutine(healthBoostPrefab, parentNetObj, localPos, localRot, healthRespawnDelay));
    }

    private void HandleSpeedBoostConsumed(NetworkObject parentNetObj, Vector3 localPos, Quaternion localRot)
    {
        StartCoroutine(RespawnCoroutine(speedBoostPrefab, parentNetObj, localPos, localRot, speedRespawnDelay));
    }

    private IEnumerator RespawnCoroutine(NetworkPrefabRef prefab, NetworkObject parentNetObj, Vector3 localPosition, Quaternion localRotation, float respawnDelay)
    {
        yield return new WaitForSeconds(respawnDelay);

        var spawned = Runner.Spawn(prefab, Vector3.zero, Quaternion.identity);

        if (spawned == null)
            yield break;

        if (parentNetObj != null)
        {
            spawned.transform.SetParent(parentNetObj.transform, worldPositionStays: false);
            spawned.transform.SetLocalPositionAndRotation(localPosition, localRotation);
        }
        else
        {
            spawned.transform.SetParent(null, worldPositionStays: true);
            spawned.transform.SetPositionAndRotation(localPosition, localRotation);
        }
    }
}
