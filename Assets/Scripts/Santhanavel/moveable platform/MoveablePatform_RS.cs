using UnityEngine;
using System.Collections;
public class MoveablePatform_RS : MonoBehaviour
{
    [Header("Platform Setup")]
    [SerializeField] private Transform platform;        // Spike Visual (child collider)
    [SerializeField] private Transform targetPoint;     // Visual Move Point

    [Header("Movement Speeds")]
    [Range(0, 10)]
    [SerializeField] private float forwardSpeed = 5f;
    [Range(0, 10)]
    [SerializeField] private float backwardSpeed = 3f;

    [Header("Wait Times")]
    [Range(0, 10)]
    [SerializeField] private float waitAtTarget = 1f;
    [Range(0, 10)]
    [SerializeField] private float waitAtStart = 1f;

    [Header("Player Carry Option")]
    [SerializeField] private bool carryPlayer = true;

    private Vector3 startPos;
    private Transform player;

    private void Start()
    {
        startPos = platform.position;
        StartCoroutine(MovementLoop());
    }

    private IEnumerator MovementLoop()
    {
        while (true)
        {
            yield return StartCoroutine(Move(platform, targetPoint.position, forwardSpeed));
            yield return new WaitForSeconds(waitAtTarget);

            yield return StartCoroutine(Move(platform, startPos, backwardSpeed));
            yield return new WaitForSeconds(waitAtStart);
        }
    }

    private IEnumerator Move(Transform obj, Vector3 destination, float speed)
    {
        while (Vector3.Distance(obj.position, destination) > 0.02f)
        {
            obj.position = Vector3.MoveTowards(obj.position, destination, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only react if THIS trigger belongs to the platform (child)
        if (collision.transform == platform) return;

        if (!carryPlayer) return;
        if (collision.gameObject.tag == "Player")
        {
            player = collision.gameObject.transform;
            player.parent = platform.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (player != null && player.gameObject.activeSelf)
            {
                player.transform.parent = null;
                player = null;
            }
        }
    }
}