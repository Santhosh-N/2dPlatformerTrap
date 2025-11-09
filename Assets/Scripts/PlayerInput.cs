using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Input States")]
    public bool jump;
    public bool jumpHeld;
    public float horizontal;
    public bool interact;
    public bool travel;

    [Header("Touch Joystick Settings")]
    public bool enableTouchJoystick = true;
    public float dragSensitivity = 0.02f; // smaller = more sensitive
    private Vector2 touchStartPos;
    private bool isDragging = false;
    private bool isLeftSideDrag = false;
    private float lastHorizontal = 0f; // remember last drag direction

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleKeyboardAndMouseInput(); // Editor / PC input
#else
        if (enableTouchJoystick)
            HandleTouchInput(); // Mobile input
        else
            HandleKeyboardAndMouseInput(); // fallback if touch disabled
#endif
    }

    // -------------------- KEYBOARD + MOUSE (Editor / PC) --------------------
    void HandleKeyboardAndMouseInput()
    {
        // Keyboard movement (WASD or Arrow keys)
        horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows

        // Jump
        if (Input.GetKeyDown(KeyCode.UpArrow))
            jump = true;
        jumpHeld = Input.GetKey(KeyCode.UpArrow);

        // Optional interact / travel keys
        if (Input.GetKeyDown(KeyCode.F))
            interact = true;
        if (Input.GetKeyDown(KeyCode.E))
            travel = true;

        // --- Mouse drag (simulate left screen touch drag) ---
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isLeftSideDrag = touchStartPos.x < Screen.width / 2f;
            isDragging = isLeftSideDrag;
            lastHorizontal = 0f;
        }
        else if (Input.GetMouseButton(0) && isDragging && isLeftSideDrag)
        {
            float dragDelta = (Input.mousePosition.x - touchStartPos.x) * dragSensitivity;

            if (dragDelta > 0.1f)
                lastHorizontal = 1f;
            else if (dragDelta < -0.1f)
                lastHorizontal = -1f;
            else
                lastHorizontal = 0f;

            horizontal = lastHorizontal;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            isLeftSideDrag = false;
            lastHorizontal = 0f;
        }
    }

    // -------------------- TOUCH (Mobile) --------------------
    void HandleTouchInput()
    {
        horizontal = 0f;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isLeftSideDrag = touchStartPos.x < Screen.width / 2f;
                    isDragging = isLeftSideDrag;
                    lastHorizontal = 0f;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isDragging && isLeftSideDrag)
                    {
                        float dragDelta = (touch.position.x - touchStartPos.x) * dragSensitivity;

                        if (dragDelta > 0.1f)
                            lastHorizontal = 1f;
                        else if (dragDelta < -0.1f)
                            lastHorizontal = -1f;
                        else
                            lastHorizontal = 0f;

                        horizontal = lastHorizontal;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    isLeftSideDrag = false;
                    lastHorizontal = 0f;
                    horizontal = 0f;
                    break;
            }
        }

        // Optional: second finger = jump
        if (Input.touchCount == 2)
        {
            if (!jump)
                jump = true;
            jumpHeld = true;
        }
        else
        {
            jumpHeld = false;
        }
    }
}
