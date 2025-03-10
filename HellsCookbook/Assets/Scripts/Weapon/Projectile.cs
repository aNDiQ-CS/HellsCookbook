using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1; // ���� �������
    public GameObject hitEffect; // ������ ���������

    void OnCollisionEnter(Collision collision)
    {
        // ��������� �����
        if (collision.gameObject.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }

        // ����� �������
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}