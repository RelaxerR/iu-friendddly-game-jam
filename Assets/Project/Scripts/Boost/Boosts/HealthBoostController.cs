using Project.Scripts.Boost.Settings;
using UnityEngine;
using Fusion;
using System;

/// <summary>
/// Контроллер буста здоровья.
/// </summary>
[RequireComponent(typeof(NetworkObject))]
public class HealthBoostController : NetworkBehaviour
{
    public static event Action<Vector3> OnBoostConsumed;

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

        Vector3 spawnPosition = transform.position;
        Runner.Despawn(Object);
        OnBoostConsumed?.Invoke(spawnPosition);
    }
}
