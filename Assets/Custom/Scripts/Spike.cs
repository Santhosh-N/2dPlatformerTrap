using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            var dir = collision.transform.position.x > transform.position.x ? 1f : -1f;
            var force = new Vector3(-5, 10, 0);
            collision.GetComponent<Player>().Damage(force);
          // collision.GetComponent<Player>().Die();
        }
    }
}
