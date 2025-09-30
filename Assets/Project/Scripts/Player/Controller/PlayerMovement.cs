using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;       // Скорость передвижения
    [SerializeField] private float jumpHeight = 1.5f;    // Высота прыжка
    [SerializeField] private float gravity = -9.81f;     // Сила гравитации
    [SerializeField] private Camera Camera;  // Ссылка на камеру
    [SerializeField] private float slideSpeed = 5f;      // Скорость скатывания по наклону
    [SerializeField] private float slopeLimit = 45f;     // Максимальный угол, на котором персонаж не скользит

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Camera = Camera.main;
            Camera.GetComponent<PlayerCamera>().Target = transform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);
        Vector3 move = inputDir.magnitude > 0.01f ? inputDir.normalized : Vector3.zero;

        // Движение относительно камеры
        move = Camera.transform.TransformDirection(move);
        move.y = 0f;

        velocity.x = move.x * moveSpeed;
        velocity.z = move.z * moveSpeed;

        velocity.y += gravity * Runner.DeltaTime;

        // Скатывание по наклонам
        if (isGrounded)
        {
            Vector3 hitNormal = Vector3.up;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height / 2f + 0.5f))
            {
                hitNormal = hit.normal;
            }

            float slopeAngle = Vector3.Angle(hitNormal, Vector3.up);
            if (slopeAngle > slopeLimit)
            {
                Vector3 slideDir = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                velocity += slideDir * slideSpeed * Runner.DeltaTime;
            }
        }

        controller.Move(velocity * Runner.DeltaTime);
    }
}
