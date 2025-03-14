using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Header("Melee Settings")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float attackDelay = 1.5f;

    [Header("���������")]
    [SerializeField] private float activationDistance = 5f;
    [SerializeField] private SphereCollider activationTrigger;    

    protected override void Start()
    {
        base.Start();
        activationTrigger.radius = activationDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            Activate();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            Deactivate();
        }
    }

    private float lastAttackTime;

    protected override void HandleAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {                
                animator.SetTrigger("Attack");                
                lastAttackTime = Time.time;
                StartCoroutine(DealDamage());
            }
        }
    }

    private IEnumerator DealDamage()
    {
        yield return new WaitForSeconds(attackDelay);
        RaycastHit[] sphereHits = Physics.SphereCastAll(
            origin: transform.position,
            radius: 1f,
            direction: transform.forward,
            maxDistance: 10f
        );

        foreach ( RaycastHit hit in sphereHits )
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (hit.collider.TryGetComponent<PlayerHealth>(out var health))
                {
                    health.TakeDamage(damage);
                }                
            }
        }
        /*Debug.Log("������� �����");

        // ��������� ��� ������������ � ���������
        Vector3 start = transform.position;
        Vector3 direction = transform.forward;
        float radius = attackRange;
        float maxDistance = attackRange + 3f;

        // ������������ ���� � Scene View
        Debug.DrawRay(start, direction * maxDistance, Color.red, 2f);        

        RaycastHit hit;
        bool isHit = Physics.SphereCast(
            origin: start,
            radius: radius,
            direction: direction,
            hitInfo: out hit,
            maxDistance: maxDistance,
            layerMask: playerMask,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        if (isHit)
        {
            Debug.Log($"��������� � ������: {hit.collider.name}", hit.collider.gameObject);

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("����� � ������!");
                if (hit.collider.TryGetComponent<PlayerHealth>(out var health))
                {
                    health.TakeDamage(damage);
                }
                else
                {
                    Debug.LogError("� ������ ����������� ��������� PlayerHealth");
                }
            }
            else
            {
                Debug.Log("����� � ������ � �����: " + hit.collider.tag);
            }
        }
        else
        {
            Debug.Log("������");
        }*/
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}