using UnityEngine;

public class SimpleShooter : MonoBehaviour
{
    [Header("Основные настройки")]
    public GameObject projectilePrefab; 
    public Transform firePoint;        
    public float fireRate = 0.5f;      
    public float projectileSpeed = 10f;

    private float nextFireTime;

    void Update()
    {        
        if (Input.GetMouseButton(0))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );
        
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * projectileSpeed, ForceMode.Impulse);
    }
}