using System;
using Fusion;
using UnityEngine;
using Project.Scripts.Boost.Settings;

[RequireComponent(typeof(NetworkObject))]
public class SpeedBoostController : NetworkBehaviour
{
    public static event Action<NetworkObject, Vector3, Quaternion> OnBoostConsumed;

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

        var parentTransform = transform.parent;
        NetworkObject parentNetObj = null;

        if (parentTransform != null)
            parentNetObj = parentTransform.GetComponentInParent<NetworkObject>();

        Vector3 localPosition;
        Quaternion localRotation;

        if (parentTransform != null)
        {
            localPosition = parentTransform.InverseTransformPoint(transform.position);
            localRotation = Quaternion.Inverse(parentTransform.rotation) * transform.rotation;
        }
        else
        {
            transform.GetPositionAndRotation(out localPosition, out localRotation);
        }

        Runner.Despawn(Object);

        OnBoostConsumed?.Invoke(parentNetObj, localPosition, localRotation);
    }
}
