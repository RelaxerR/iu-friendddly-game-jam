using System;
using Fusion;
using UnityEngine;
using Project.Scripts.Boost.Settings;

[RequireComponent(typeof(NetworkObject))]
public class HealthBoostController : NetworkBehaviour
{
    public static event Action<NetworkObject, Vector3, Quaternion> OnBoostConsumed;

    [SerializeField] private HealthBoostSettings healthBoostSettings;

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority)
            return;

        var health = other.GetComponentInParent<Health>();
        if (health == null || health.IsDead)
            return;

        var pm = other.GetComponentInParent<PlayerMovement>();

        bool hasSpeedActive = false;
        if (pm != null && pm.NetworkedSpeedMultiplier != 1f && Runner.SimulationTime < pm.BoostEndTime)
            hasSpeedActive = true;

        if (hasSpeedActive)
        {
            float currentHp = health.NetworkedHealth;
            health.DealDamageRpc(currentHp, PlayerRef.None);
        }
        else
        {
            health.HealRpc(healthBoostSettings.HealAmount);
        }

        var parentTransform = transform.parent;
        NetworkObject parentNetObj = null;

        if (parentTransform != null)
            parentNetObj = parentTransform.GetComponentInParent<NetworkObject>();

        Vector3 localPosisiton;
        Quaternion localRotation;

        if (parentTransform != null)
        {
            localPosisiton = parentTransform.InverseTransformPoint(transform.position);
            localRotation = Quaternion.Inverse(parentTransform.rotation) * transform.rotation;
        }
        else
        {
            transform.GetPositionAndRotation(out localPosisiton, out localRotation);
        }

        Runner.Despawn(Object);
        OnBoostConsumed?.Invoke(parentNetObj, localPosisiton, localRotation);
    }
}
