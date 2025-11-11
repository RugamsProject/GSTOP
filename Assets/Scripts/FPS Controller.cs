using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    
    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;
    public float maxLookAngle = 80f;

    // Компоненты
    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction runAction;

    // Переменные движения
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning = false;
    
    // Переменные для вращения камеры
    private float xRotation = 0f;

    void Awake()
    {
        // Получаем компоненты
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        
        // Получаем Input Actions
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];
    }

    void Start()
    {
        // Заблокировать и скрыть курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Убедиться что камера назначена
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleJump();
        ApplyGravity();
    }

    void HandleMovement()
    {
        // Проверяем бег (Shift)
        isRunning = runAction.IsPressed();
        
        // Получаем ввод движения
        Vector2 input = moveAction.ReadValue<Vector2>();
        
        // Вычисляем скорость
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Двигаем персонажа
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleLook()
    {
        // Получаем ввод мыши
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        
        // Вращаем персонажа по Y (горизонтально)
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        
        // Вращаем камеру по X (вертикально)
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleJump()
    {
        // Проверяем землю
        isGrounded = controller.isGrounded;
        
        // Сбрасываем скорость падения когда на земле
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        // Прыжок
        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        // Применяем гравитацию
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Опционально: для разблокировки курсора
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}