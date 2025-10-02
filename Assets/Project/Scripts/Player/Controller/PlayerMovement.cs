using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerForces))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float slideSpeed = 300f;
    [SerializeField] private float slopeLimit = 30f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
            return;

        // Хак для CharacterController при спавне
        controller.enabled = false;
        transform.position = transform.position;
        controller.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void FixedUpdateNetwork()
    {
        // берём ввод, если он отсутствует — выходим
        if (!GetInput(out NetworkInputData inputData))
            return;

        // наземье и прыжок
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        if (inputData.JumpPressed && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // строим basis из угла yaw
        float yawRad = inputData.CameraYaw * Mathf.Deg2Rad;
        Vector3 forward = new Vector3(
          Mathf.Sin(yawRad),
          0f,
          Mathf.Cos(yawRad)
        );
        Vector3 right = new Vector3(forward.z, 0f, -forward.x);

        Vector3 inputDir = new Vector3(inputData.Direction.x, 0f, inputData.Direction.y);
        Vector3 move = inputDir.x * right + inputDir.z * forward;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        // устанавливаем горизонтальные скорости
        velocity.x = move.x * moveSpeed;
        velocity.z = move.z * moveSpeed;
        velocity.y += gravity * Runner.DeltaTime;

        // скатывание по крутым склонам
        if (isGrounded)
        {
            Vector3 hitNormal = Vector3.up;
            if (Physics.Raycast(
                transform.position,
                Vector3.down,
                out var hit,
                controller.height / 2f + 0.5f))
            {
                hitNormal = hit.normal;
            }

            float angle = Vector3.Angle(hitNormal, Vector3.up);
            if (angle > slopeLimit)
            {
                Vector3 slideDir = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                velocity += slideDir * (slideSpeed * Runner.DeltaTime);
            }
        }

        // внешние силы
        Vector3 extraForces = GetComponent<PlayerForces>()?.ConsumeForces() ?? Vector3.zero;
        velocity += extraForces;

        controller.Move(velocity * Runner.DeltaTime);
    }
}
