using System.Collections;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkBehaviour))]
public class RaycastAttack : NetworkBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damage = 17f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float visualDuration = 0.3f; // увеличена длительность луча
    [SerializeField] private float fireRate = 0.5f; // промежуток между выстрелами (сек)

    [Header("Line Renderer (optional)")]
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private float lineWidth = 0.02f;

    private Camera playerCamera;
    private LineRenderer lineRendererInstance;

    private float lastFireTime = -Mathf.Infinity;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    public override void Spawned()
    {
        enabled = Object.HasInputAuthority;

        if (lineRendererPrefab != null)
        {
            lineRendererInstance = Instantiate(lineRendererPrefab, transform);
            SetupLineRenderer(lineRendererInstance);
            lineRendererInstance.enabled = false;
        }
        else
        {
            var go = new GameObject("RaycastVisual_LR");
            go.transform.SetParent(transform, false);
            lineRendererInstance = go.AddComponent<LineRenderer>();
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
        if (!Object.HasInputAuthority) return;

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
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogWarning("Player camera not found.");
                return;
            }
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 origin = ray.origin + ray.direction * 0.1f;
        RaycastHit hit;

        Vector3 targetPoint = origin + ray.direction * maxDistance;
        if (Physics.Raycast(origin, ray.direction, out hit, maxDistance))
        {
            targetPoint = hit.point;

            if (hit.transform.TryGetComponent<Health>(out var health))
            {
                PlayerRef attacker = Object.InputAuthority;
                health.DealDamageRpc(damage, attacker);
            }
        }

        if (lineRendererInstance != null)
            StartCoroutine(ShowLine(origin, targetPoint, visualDuration));
        else
            Debug.DrawLine(origin, targetPoint, new Color(1f, 0.5f, 0f), visualDuration);
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
