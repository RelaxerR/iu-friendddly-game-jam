using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 100f; // Чувствительность мыши
    [SerializeField] private float maxLookAngle = 85f; // Максимальный угол обзора
    [SerializeField] private Vector3 offset = new(0, 0.5f, 0); // Смещение камеры для нужного расположения

    public Transform Target;
    private float xRotation = 0f;
    private float yRotation;

    public void LateUpdate()
    {
        if (Target == null)
            return;

        transform.position = Target.position + offset;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Поворот камерой вверх/вниз
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}