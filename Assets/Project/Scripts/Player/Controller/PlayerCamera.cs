using Fusion;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;

    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 100f; // Чувствительность мыши
    [SerializeField] private float maxLookAngle = 85f; // Максимальный угол обзора

    private float xRotation = 0f;

    public override void Spawned()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public override void FixedUpdateNetwork()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Поворот камерой вверх/вниз
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Поворот тела игрока (влево/вправо)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}