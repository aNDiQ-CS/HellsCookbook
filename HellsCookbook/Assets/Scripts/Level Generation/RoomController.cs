using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RoomController : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] protected GameObject barrierPrefab;
    [SerializeField] protected Vector3 barrierSize = new Vector3(10, 5, 0.2f);

    [Header("Effects")]
    [SerializeField] protected ParticleSystem barrierActivateEffect;
    [SerializeField] protected ParticleSystem barrierDeactivateEffect;

    protected List<Enemy> enemies = new List<Enemy>();
    protected GameObject currentBarrier;
    protected bool isActive;

    void Start()
    {
        FindEnemiesInRoom();
        CreateBarrier();
        barrierPrefab.SetActive(false);
        Debug.Log(enemies.Count);
    }

    protected void FindEnemiesInRoom()
    {
        Collider[] colliders = Physics.OverlapBox(
            transform.position,
            GetComponent<BoxCollider>().size / 2,
            Quaternion.identity,
            LayerMask.GetMask("Enemy")
        );

        foreach (Collider col in colliders)
        {            
            if (col.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemies.Add(enemy);                
                enemy.EnemyHealth.OnDeath += HandleEnemyDeath;                
            }
        }
    }

    protected void CreateBarrier()
    {
        currentBarrier = Instantiate(barrierPrefab, transform);
        currentBarrier.transform.localScale = barrierSize;
        /*currentBarrier.GetComponent<Renderer>().material.SetColor("_BaseColor",
            new Color(0, 0.5f, 1, 0.3f));*/
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive && enemies.Count > 0)
        {
            ActivateBarrier();
        }
    }

    protected void ActivateBarrier()
    {
        isActive = true;
        currentBarrier.SetActive(true);
        barrierActivateEffect.Play();

        foreach (Enemy enemy in enemies)
            enemy.Activate();
    }

    protected void HandleEnemyDeath()
    {
        Debug.Log("Кто-то умер");        
        enemies.RemoveAll(e => e == null || e.EnemyHealth.IsDead);
        Debug.Log(enemies.Count);
        if (enemies.Count == 0)
            DeactivateBarrier();
    }

    virtual protected void DeactivateBarrier()
    {
        Debug.Log("Все умерли, убираем барьер");
        isActive = false;
        StartCoroutine(AnimateBarrierDisappear());
        barrierDeactivateEffect.Play();
    }

    protected IEnumerator AnimateBarrierDisappear()
    {
        /*Material[] mats = currentBarrier.GetComponentsInChildren<Renderer>().material;*/
        Renderer[] renders = currentBarrier.GetComponentsInChildren<Renderer>();
        float duration = 1f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            foreach (Renderer r in renders)
            {
                Material mat = r.material;
                mat.SetFloat("_Alpha", Mathf.Lerp(0.3f, 0f, t / duration));
                yield return null;
            }            
        }
        currentBarrier.SetActive(false);
    }
}