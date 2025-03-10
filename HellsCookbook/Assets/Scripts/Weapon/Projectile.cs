using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1; // Урон снаряда
    public GameObject hitEffect; // Эффект попадания

    void OnCollisionEnter(Collision collision)
    {
        // Нанесение урона
        if (collision.gameObject.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }

        // Спавн эффекта
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}