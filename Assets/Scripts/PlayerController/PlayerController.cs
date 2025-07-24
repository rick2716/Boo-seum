using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private PlayerControls inputActions;
    [SerializeField] private bool isJumpPressed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerControls();

        inputActions.Player.Jump.performed += OnJumpPressed;
        inputActions.Player.Jump.canceled += OnJumpReleased;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        isJumpPressed = true;
    }

    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        isJumpPressed = false;
    }

    private void FixedUpdate()
    {
        if (isJumpPressed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
        }
    }
}
