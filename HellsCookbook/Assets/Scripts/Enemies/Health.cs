using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    private EnemyHealth health;
    private bool isDead = false;

    public bool IsDead { get { return isDead; } }

    public event System.Action OnDeath;
    public int MaxHealth
    {
        get { return maxHealth; }
        private set { }
    }

    void Awake()
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
        isDead = true;
        OnDeath?.Invoke();        
        Destroy(gameObject);
    }
}