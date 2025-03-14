using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityTime = 1f;

    [Header("Regeneration")]
    [SerializeField] private float regenDelay = 20f;
    [SerializeField] private float regenRate = 0.05f;
    [SerializeField] private int regenAmount = 1;

    [Header("Visual Effects")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image damageFlash;
    [SerializeField] private Color flashColor = new Color(1, 0, 0, 0.3f);

    [Header("Audio")]
    [SerializeField] private AudioSource damageSound;

    private int currentHealth;
    private float lastDamageTime;
    private bool isDead;

    private void Update()
    {
        /*// Для тестирования
        if (Input.GetKeyDown(KeyCode.T))
            GetComponent<PlayerHealth>().TakeDamage(10);

        if (Input.GetKeyDown(KeyCode.H))
            GetComponent<PlayerHealth>().Heal(20);*/

        if (Time.time - lastDamageTime > regenDelay && currentHealth < maxHealth)
        {
            Heal(regenAmount);
        }
    }

    void Start()
    {
        InitializeHealth();
        UpdateHealthUI();
    }

    void InitializeHealth()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        damageFlash.color = Color.clear;
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - lastDamageTime < invincibilityTime || isDead) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        lastDamageTime = Time.time;

        damageSound.Play();
        StartCoroutine(ShowDamageEffect());
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }        
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    System.Collections.IEnumerator ShowDamageEffect()
    {
        damageFlash.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        damageFlash.color = Color.clear;
    }

    void UpdateHealthUI()
    {
        healthSlider.value = currentHealth;
        
        float healthPercent = (float)currentHealth / maxHealth;
        healthSlider.fillRect.GetComponent<Image>().color =
            Color.Lerp(Color.red, Color.green, healthPercent);
    }

    void Die()
    {
        isDead = true;        
        Debug.Log("Player Died!");
        SceneManager.LoadScene("DeathScreen");
    }
}