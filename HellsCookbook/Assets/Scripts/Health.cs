using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    private EnemyHealth health;    

    public int MaxHealth
    {
        get { return maxHealth; }
        private set { }
    }

    void Start()
    {
        currentHealth = maxHealth;
        health = GetComponent<EnemyHealth>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;        
        float healthPercentage = currentHealth / (float)maxHealth;
        health.SetHealthGlowing(healthPercentage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Анимация смерти или уничтожение объекта
        Destroy(gameObject);
    }
}