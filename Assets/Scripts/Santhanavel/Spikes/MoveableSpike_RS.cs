using UnityEngine;
using System.Collections;
public class MoveableSpike_RS : MonoBehaviour
{
    [SerializeField] private Transform moveableSpike;
    [SerializeField] private Transform targetPointToMove;

    [Range(0, 10)]
    [SerializeField] private float moveSpeed = 5f;

    [Range(0, 10)]
    [SerializeField] private float returnDelay = 5f;

    [Range(0, 10)]
    [SerializeField] private float delayAtStart = 1f;

    [SerializeField] private bool returnToStart = false;

    private Vector3 initialPos;
    private bool isMoving = false;

    private void Start()
    {
        initialPos = moveableSpike.position;
    }

    public void ActivateSpike()
    {
        if (!isMoving)
            StartCoroutine(MoveSpikeSequence());
    }

    private IEnumerator MoveSpikeSequence()
    {
        isMoving = true;

        if (delayAtStart > 0)
            yield return new WaitForSeconds(delayAtStart);

        // Move to target
        yield return StartCoroutine(MoveToPosition(moveableSpike, targetPointToMove.position));

        // Return
        if (returnToStart)
        {
            yield return new WaitForSeconds(returnDelay);
            yield return StartCoroutine(MoveToPosition(moveableSpike, initialPos));
        }

        isMoving = false;
    }

    private IEnumerator MoveToPosition(Transform obj, Vector3 target)
    {
        while (Vector3.Distance(obj.position, target) > 0.05f)
        {
            obj.position = Vector3.MoveTowards(obj.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        obj.position = target;
    }
}
