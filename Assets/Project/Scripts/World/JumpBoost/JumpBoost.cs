using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpBoostPlatform : NetworkBehaviour
{
    [SerializeField] private float jumpHeight = 45f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerForces forces))
        {
            // v = sqrt(2 * g * h)
            float g = 9.81f;
            float jumpVelocity = Mathf.Sqrt(2f * g * jumpHeight);

            forces.AddForce(Vector3.up * jumpVelocity);
        }
    }
}
