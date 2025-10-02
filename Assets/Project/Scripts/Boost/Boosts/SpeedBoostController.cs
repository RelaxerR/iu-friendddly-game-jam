using System;
using Fusion;
using UnityEngine;
using Project.Scripts.Boost.Settings;

[RequireComponent(typeof(NetworkObject))]
public class SpeedBoostController : NetworkBehaviour
{
    public static event Action<Vector3, Quaternion> OnBoostConsumed;

    [SerializeField] private SpeedBoostSettings speedBoostSettings;

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority)
            return;

        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null)
            return;

        pm.Rpc_ApplySpeedBoost(
            speedBoostSettings.SpeedMultiplier,
            speedBoostSettings.Duration
        );

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        Runner.Despawn(Object);

        OnBoostConsumed?.Invoke(pos, rot);
    }
}
