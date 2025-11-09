using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public string blastEffectTag = "Blast"; // Use pooling tag

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            Explode();
            return;
        }

        if (other.name == "Ground")
        {
            Explode();
        }
    }

    void Explode()
    {
        if (!string.IsNullOrEmpty(blastEffectTag))
        {
            GameObject fx = ObjectPool.Instance.SpawnFromPool(blastEffectTag, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false); // Return bullet to pool
    }
}
