using Fusion;
using UnityEngine;

public class RaycastAttack : NetworkBehaviour
{
    public float Damage = 10;
    public PlayerMovement PlayerMovement;

    void Update()
    {
        if (HasStateAuthority == false)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = PlayerMovement.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            ray.origin += PlayerMovement.Camera.transform.forward * 0.1f;
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.orange, 1f);

            if (Physics.Raycast(ray, out var hit, 100f))
            {
                if (hit.transform.TryGetComponent<Health>(out var health))
                {
                    health.DealDamageRpc(Damage);
                }
            }
        }
    }
}