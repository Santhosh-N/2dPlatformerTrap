using UnityEngine;

public class DoorTrap : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] Transform doorPosition;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            door.transform.position = doorPosition.position;
            Destroy(gameObject);
        }
    }
}
