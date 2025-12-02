using UnityEngine;
using UnityEngine.Events;

public class MoveableSpikeTrigger_RS : MonoBehaviour
{
    [SerializeField] private MoveableSpike_RS spike;
    [SerializeField] private UnityEvent triggerEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            spike.ActivateSpike();
            triggerEvent?.Invoke();
        }
    }
}