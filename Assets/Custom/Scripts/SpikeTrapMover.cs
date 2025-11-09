using UnityEngine;

public class SpikeTrapMover : MonoBehaviour
{
    [Tooltip("The point where the spike starts.")]
    public Vector3 startPosition;
    [Tooltip("The target position the spike moves to when triggered.")]
    public Vector3 targetPosition;
    [Tooltip("Speed of movement (units/second).")]
    public float moveSpeed = 5f;
    [Tooltip("Should the spike move back to start after reaching target?")]
    public bool returnToStart = true;
    [Tooltip("Delay at the target before moving back (seconds).")]
    public float delayAtTarget = 1f;

    private bool triggered = false;
    private bool movingToTarget = false;
    private float delayTimer = 0f;

    void Start()
    {
        // initialize
        transform.position = startPosition;
    }

    void Update()
    {
        if (triggered)
        {
            if (movingToTarget)
            {
                // Move towards the target position
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // Check if reached target
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    movingToTarget = false;
                    delayTimer = delayAtTarget;
                }
            }
            else if (returnToStart && delayTimer <= 0f)
            {
                // Move back towards the start position
                transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);

                // You could choose to disable further movement once back:
                if (Vector3.Distance(transform.position, startPosition) < 0.01f)
                {
                    triggered = false; // reset
                }
            }
            else
            {
                // counting delay
                delayTimer -= Time.deltaTime;
            }
        }
    }

    // Call this method to trigger the spike moving
    public void TriggerSpike()
    {
        if (!triggered)
        {
            triggered = true;
            movingToTarget = true;
        }
    }

    // Example: trigger when player enters collider
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerSpike();
        }
    }
}
