using Fusion;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [Header("Health Settings")]
    [Networked] public float NetworkedHealth { get; set; } = 100f;
    [Networked] public bool IsDead { get; private set; }
    [Networked] public int DeathCount { get; private set; }

    private CharacterController controller;

    public delegate void DeathHandler(Health victim, PlayerRef killer);
    public event DeathHandler OnDeath;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage, PlayerRef attacker)
    {
        if (IsDead)
            return;

        NetworkedHealth -= damage;

        if (NetworkedHealth <= 0f)
        {
            NetworkedHealth = 0f;
            IsDead = true;
            DeathCount++;
            OnDeath?.Invoke(this, attacker);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (IsDead)
            DoRespawn();
    }

    private void DoRespawn()
    {
        var points = SpawnManager.Instance?.SpawnPoints;
        if (points == null || points.Length == 0)
            return;

        var sp = points[Random.Range(0, points.Length)];

        controller.enabled = false;
        transform.position = sp.position;
        transform.rotation = sp.rotation;
        controller.enabled = true;

        NetworkedHealth = 100f;
        IsDead = false;

        var camera = GetComponentInChildren<PlayerCamera>();
        if (camera != null)
        {
            camera.Rpc_RespawnCamera();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void HealRpc(float amount)
    {
        if (IsDead) return;

        NetworkedHealth += amount;
        if (NetworkedHealth > 100f)
            NetworkedHealth = 100f;
    }
}
