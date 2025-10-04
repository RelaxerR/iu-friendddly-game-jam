using System.Collections;
using Fusion;
using UnityEngine;
using Project.Scripts.Bootstrap;

[RequireComponent(typeof(NetworkBehaviour))]
public class RaycastAttack : NetworkBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damage = 17f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float visualDuration = 0.3f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Line Renderer (optional)")]
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private float lineWidth = 0.03f;

    [Header("Muzzle Settings")]
    [SerializeField] private Transform muzzleTransform;

    private Camera playerCamera;
    private LineRenderer lineRendererInstance;
    private float lastFireTime = -Mathf.Infinity;
    private GameModeManager modeManager;

    public override void Spawned()
    {
        enabled = Object.HasInputAuthority;

        if (Object.HasInputAuthority)
        {
            playerCamera = GetComponentInChildren<Camera>();
            modeManager = FindObjectOfType<GameModeManager>();

            if (lineRendererPrefab != null)
                lineRendererInstance = Instantiate(lineRendererPrefab, transform);
            else
                lineRendererInstance = new GameObject("RaycastVisual_LR", typeof(LineRenderer))
                    .GetComponent<LineRenderer>();

            lineRendererInstance.transform.SetParent(transform, false);
            SetupLineRenderer(lineRendererInstance);
            lineRendererInstance.enabled = false;
        }
    }

    private void SetupLineRenderer(LineRenderer lr)
    {
        lr.positionCount = 2;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = true;

        if (lr.material == null)
        {
            var mat = new Material(Shader.Find("Unlit/Color"));
            mat.SetColor("_Color", new Color(1f, 0.5f, 0f, 1f));
            lr.material = mat;
        }
    }

    private void Update()
    {
        if (!Object.HasInputAuthority)
            return;

        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + fireRate)
        {
            lastFireTime = Time.time;
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Player camera not found.");
            return;
        }

        Ray camRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 camOrigin = camRay.origin;
        Vector3 camDirection = camRay.direction;

        Vector3 visualOrigin = muzzleTransform != null
            ? muzzleTransform.position
            : camOrigin + camDirection * 0.1f;

        Vector3 hitPoint = camOrigin + camDirection * maxDistance;
        RaycastHit hitInfo;
        if (Physics.Raycast(camOrigin, camDirection, out hitInfo, maxDistance))
        {
            hitPoint = hitInfo.point;

            if (hitInfo.transform.TryGetComponent<Health>(out var targetHealth))
            {
                PlayerRef attacker = Object.InputAuthority;
                bool isGreen = modeManager != null && modeManager.CurrentMode == GameModeManager.GameMode.GreenTime;

                if (isGreen)
                {
                    if (TryGetComponent<Health>(out var selfHealth))
                        selfHealth.DealDamageRpc(damage, attacker);
                }
                else
                {
                    targetHealth.DealDamageRpc(damage, attacker);
                }
            }
        }

        if (lineRendererInstance != null)
            StartCoroutine(ShowLine(visualOrigin, hitPoint, visualDuration));
        else
            Debug.DrawLine(visualOrigin, hitPoint, new Color(1f, 0.5f, 0f), visualDuration);
    }

    private IEnumerator ShowLine(Vector3 from, Vector3 to, float duration)
    {
        lineRendererInstance.SetPosition(0, from);
        lineRendererInstance.SetPosition(1, to);
        lineRendererInstance.enabled = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        lineRendererInstance.enabled = false;
    }
}
