using UnityEngine;
using Fusion;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 250f;
    [SerializeField] private float maxLookAngle = 45f;
    [SerializeField] private Transform PlayerBody;

    private Camera cam;
    private AudioListener audioListener;

    // спортсмены для оффсета
    private Vector3 _startLocalPos;
    private Quaternion _startLocalRot;

    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        audioListener = GetComponent<AudioListener>();

        // сохраняем исходный локальный оффсет камеры
        _startLocalPos = transform.localPosition;
        _startLocalRot = transform.localRotation;
    }

    public override void Spawned()
    {
        // только локальному игроку оставляем камеру и слушатель звука
        if (!Object.HasInputAuthority)
        {
            cam.enabled = false;
            if (audioListener) audioListener.enabled = false;
            return;
        }

        // инициализируем Yaw по телу
        yRotation = PlayerBody.rotation.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (!Object.HasInputAuthority) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        yRotation += mouseX;
        PlayerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    // RPC с StateAuthority → только на том клиенте, который владеет входом
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void Rpc_RespawnCamera()
    {
        // мгновенно вернуть локальные оффсеты
        transform.localPosition = _startLocalPos;
        transform.localRotation = _startLocalRot;

        // сброс углов просмотра
        xRotation = 0f;
        yRotation = PlayerBody.rotation.eulerAngles.y;
    }
}
