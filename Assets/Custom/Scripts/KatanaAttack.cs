using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class KatanaAttack : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            return;
        }
    }
}
