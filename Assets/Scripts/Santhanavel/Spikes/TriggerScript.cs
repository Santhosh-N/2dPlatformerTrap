using UnityEngine;
using UnityEngine.Events;

public class TriggerScript : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            triggerEvent?.Invoke();
        }
    }
}