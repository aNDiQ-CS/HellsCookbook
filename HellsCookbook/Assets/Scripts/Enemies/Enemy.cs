using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float activationRadius = 10f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected Animator animator;

    protected Transform player;
    protected NavMeshAgent agent;
    protected bool isActive;
    protected Health enemyHealth;

    public Health EnemyHealth { get { return enemyHealth; } }

    protected void Awake()
    {
        enemyHealth = GetComponentInChildren<Health>();
        enemyHealth.OnDeath += Die;
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;        
        agent.speed = moveSpeed;        
    }

    protected virtual void Update()
    {
        if (!isActive) return;
        HandleMovement();
        HandleAttack();        
    }

    protected virtual void HandleMovement()
    {
        agent.SetDestination(player.position);
        RotateTowardsPlayer();
    }

    protected virtual void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    protected abstract void HandleAttack();

    public virtual void Activate()
    {
        isActive = true;
        agent.enabled = true;
        animator.enabled = true;
        Debug.Log("Активирован");
    }

    public virtual void Deactivate()
    {
        isActive = false;
        agent.enabled = false;
        animator.enabled = false;
        Debug.Log("Спим");
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}