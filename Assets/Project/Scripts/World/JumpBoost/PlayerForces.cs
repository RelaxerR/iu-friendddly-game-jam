using UnityEngine;

public class PlayerForces : MonoBehaviour
{
    private Vector3 externalForces = Vector3.zero;

    public void AddForce(Vector3 force)
    {
        externalForces += force;
    }

    public Vector3 ConsumeForces()
    {
        Vector3 f = externalForces;
        externalForces = Vector3.zero;
        return f;
    }
}
