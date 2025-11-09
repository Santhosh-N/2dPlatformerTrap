using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IDamageable
{

    [Header("Health")]
    public int maxHealth = 3;
    int health;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject destroyEffect;

    void Awake()
    {
        health = maxHealth;
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = health;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        OnHit();
        if (health <= 0) Die();
    }

    void OnHit() { UpdateHealth(health); }
    void Die()
    {
        Destroy(gameObject);
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
    }

    public void UpdateHealth(float currentHealth)
    {
        if (slider != null)
            slider.value = currentHealth;
    }
}
