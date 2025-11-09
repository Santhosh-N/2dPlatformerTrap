using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; // for BaseEventData handlers

public class Player : MonoBehaviour, IDamageable
{
    public static Player Instance;

    void Awake() { Instance = this; }

    public enum State
    {
        None, Normal, Traveling, Hurt, Dead
    }

    [Header("Movement Settings")]
    public float speed;
    public float jumpForce;
    public float jumpHoldForce;
    public float jumpHoldDuration;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    public float groundCheckRadius;

    [Header("Travel Settings")]
    public float travelSpeed;
    public ParticleSystem travelTrail;

    [Header("Audio Clips")]
    public AudioClip jumpClip;
    public AudioClip travelClip;
    public AudioClip hurtClip;
    public AudioClip dieClip;
    public AudioClip teleportClip;

    [Header("References")]
    [SerializeField] private PlayerGun playerGun; // 🔫 Gun reference

    [Header("Mobile Controls")]
    public bool enableMobileControls = true;

    [Space]
    [SerializeField] bool isOnGround;
    [SerializeField] bool isJumping;

    float jumpCounter;
    Torch currentTorch;
    Transform travelTarget;
    int originLayer;
    public State state = State.None;

    SpriteRenderer spriteRenderer;
    SpriteRenderer gunSpriteRenderer;

    Rigidbody2D rb;
    Animator animator;
    AudioSource audioSource;
    PlayerInput input;
    TriggerArea2D triggerArea;

    // -1 = left, 0 = none, 1 = right
    float mobileHorizontal = 0f;

    private Vector3 previousPosition; // 🟢 store last frame position

    void Start()
    {
        // existing setup
        spriteRenderer = GetComponent<SpriteRenderer>();
        gunSpriteRenderer = playerGun.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        triggerArea = GetComponentInChildren<TriggerArea2D>();

        triggerArea.TriggerEnter2D += OnInteractTriggerEnter;
        triggerArea.TriggerExit2D += OnInteractTriggerExit;

        originLayer = gameObject.layer;
        state = State.Normal;

        previousPosition = transform.position; // initialize previous pos
    }

    void Update()
    {
        if (state == State.Dead) return;

        GroundCheck();
        ApplyAnimation();

        if (input.interact)
        {
            if (currentTorch != null && state != State.Traveling)
            {
                currentTorch.Toggle();
                currentTorch.ShowHint();
            }
            input.interact = false;
        }

        if (input.travel)
        {
            if (currentTorch != null)
            {
                if (currentTorch.NearestTorch != null && currentTorch.isOn)
                    Travel(currentTorch.NearestTorch.transform);
            }
            input.travel = false;
        }

        if (state == State.Traveling)
        {
            var cols = Physics2D.OverlapCircleAll(transform.position, 1.5f, 1 << LayerMask.NameToLayer("Enemy"));
            if (cols.Length > 0)
            {
                foreach (var item in cols)
                {
                    var dmg = item.GetComponent<IDamageable>();
                    if (dmg != null)
                        dmg.TakeDamage(1);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (state == State.Normal)
        {
            float currentHorizontal = (enableMobileControls && Mathf.Abs(mobileHorizontal) > 0.001f)
                ? mobileHorizontal : input.horizontal;

            float velocityX = currentHorizontal * speed;

            // Jump logic (unchanged)
            if (input.jump && isOnGround && !isJumping)
            {
                input.jump = false;
                isJumping = true;
                jumpCounter = Time.time + jumpHoldDuration;
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                Utils.PlayRandomPitch(audioSource, jumpClip);
            }
            else if (isJumping)
            {
                if (input.jumpHeld)
                    rb.AddForce(new Vector2(0, jumpHoldForce), ForceMode2D.Impulse);
                if (jumpCounter < Time.time)
                    isJumping = false;
            }

            if (!isOnGround)
                input.jump = false;

            rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);

            // 🟡 Only rotate if player has moved significantly
            bool hasMoved = Mathf.Abs(transform.position.x - previousPosition.x) > 0.001f;

            if (hasMoved)
            {
                if (playerGun != null && playerGun.currentTarget != null)
                {
                    RotateCharacter(playerGun.IsFacingRight);
                    gunSpriteRenderer.flipY = !playerGun.IsFacingRight; // Flip gun sprite when facing lefT
                }
                else
                {
                    if (currentHorizontal > 0)
                    {
                        RotateCharacter(true);
                        gunSpriteRenderer.flipY = !playerGun.IsFacingRight; // Flip gun sprite when facing lefT
                    }
                    else if (currentHorizontal < 0)
                    {
                        RotateCharacter(false);
                        gunSpriteRenderer.flipY = !playerGun.IsFacingRight; // Flip gun sprite when facing lefT
                    }
                }
            }

            // update stored position
            previousPosition = transform.position;
        }
        else if (state == State.Traveling)
        {
            // traveling logic (unchanged)
            var dir = travelTarget.position - transform.position;
            if (dir.sqrMagnitude <= .5f)
            {
                state = State.Normal;
                gameObject.layer = originLayer;
                rb.linearVelocity = Vector2.zero;
                travelTrail.Stop();
                animator.SetBool("IsTraveling", false);
            }
            else
            {
                rb.linearVelocity = travelSpeed * dir.normalized;
            }
        }
    }

    // ✅ New rotation-based facing
    private void RotateCharacter(bool facingRight)
    {
        float targetY = facingRight ? 0f : 180f;
        Quaternion targetRot = Quaternion.Euler(0f, targetY, 0f);

        // Smooth rotation
        transform.rotation = targetRot;

        // Keep gun upright
        /* if (playerGun != null)
         {
             playerGun.transform.localRotation = Quaternion.identity;
         }*/
    }

    public void SetFacingDirection(bool facingRight)
    {
        RotateCharacter(facingRight);
        gunSpriteRenderer.flipY = facingRight; // Flip gun sprite when facing lefT
    }

    void GroundCheck()
    {
        var col = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        isOnGround = col != null;
    }

    void ApplyAnimation()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("VelocityY", rb.linearVelocity.y);
        animator.SetBool("IsOnGround", isOnGround);
    }

    void Travel(Transform target)
    {
        if (state != State.Normal) return;
        state = State.Traveling;

        travelTarget = target;
        gameObject.layer = LayerMask.NameToLayer("Void");
        travelTrail.Play();

        animator.SetBool("IsTraveling", true);
        Utils.PlayRandomPitch(audioSource, travelClip);
    }

    public void Damage(Vector3 force)
    {
        if (state == State.Hurt) return;
        state = State.Hurt;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        animator.SetBool("IsHurt", true);
        StartCoroutine(HurtDelay());
        Utils.PlayRandomPitch(audioSource, hurtClip);
    }

    public void TakeDamage(int amount)
    {
       
    }

    IEnumerator HurtDelay()
    {
        yield return new WaitForSeconds(0.5f);
        state = State.Normal;
        animator.SetBool("IsHurt", false);
        animator.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
    }

    public void Die()
    {
        if (state == State.Dead) return;
        state = State.Dead;
      //  rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsDead", true);
        Utils.PlayRandomPitch(audioSource, dieClip);
        GameManager.Instance.GameOver();
    }

    public void Transport()
    {
        if (state == State.Dead) return;
        state = State.Dead;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        animator.SetBool("IsTeleport", true);
        Utils.PlayRandomPitch(audioSource, teleportClip);
        GameManager.Instance.NextLevel();
    }

    void OnInteractTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Torch"))
        {
            var torch = other.GetComponent<Torch>();
            if (torch != currentTorch)
            {
                if (currentTorch != null)
                    currentTorch.HideHint();
                currentTorch = torch;
                currentTorch.ShowHint();
            }
        }
    }

    void OnInteractTriggerExit(Collider2D other)
    {
        if (other.CompareTag("Torch"))
        {
            var torch = other.GetComponent<Torch>();
            if (torch == currentTorch)
            {
                torch.HideHint();
                currentTorch = null;
            }
        }
    }

    #region UI Button / EventTrigger Methods

    public void MoveLeft(bool isPressed) => mobileHorizontal = isPressed ? -1f : 0f;
    public void MoveRight(bool isPressed) => mobileHorizontal = isPressed ? 1f : 0f;

    public void JumpButtonDown()
    {
        input.jump = true;
        input.jumpHeld = true;
    }

    public void JumpButtonUp()
    {
        input.jumpHeld = false;
    }

    public void OnEPressed() => input.travel = true;
    public void OnFPressed() => input.interact = true;
    public void OnRPressed() => GameManager.Instance.RestartCurrent();

    public void MoveLeftDown() => mobileHorizontal = -1f;
    public void MoveLeftUp() { if (mobileHorizontal < 0) mobileHorizontal = 0f; }

    public void MoveRightDown() => mobileHorizontal = 1f;
    public void MoveRightUp() { if (mobileHorizontal > 0) mobileHorizontal = 0f; }

    public void OnJumpDown() => JumpButtonDown();
    public void OnJumpUp() => JumpButtonUp();

    public void OnLeftPointerDown(BaseEventData data) => MoveLeftDown();
    public void OnLeftPointerUp(BaseEventData data) => MoveLeftUp();
    public void OnRightPointerDown(BaseEventData data) => MoveRightDown();
    public void OnRightPointerUp(BaseEventData data) => MoveRightUp();
    public void OnJumpPointerDown(BaseEventData data) => OnJumpDown();
    public void OnJumpPointerUp(BaseEventData data) => OnJumpUp();

    #endregion
}
