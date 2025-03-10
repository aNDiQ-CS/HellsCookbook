using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    private Material _glowMat;

    void Start()
    {
        // ��������� �������� ��� ������������� ���������
        _glowMat = _renderer.material;
    }

    public void SetHealthGlowing(float currentHealthPercentage)
    {
        float currentSpeed = _glowMat.GetFloat("_PulseSpeed");
        float currentGlow = _glowMat.GetFloat("_GlowIntensity");
        _glowMat.SetFloat("_PulseSpeed", 10 - currentHealthPercentage * 10);
        _glowMat.SetFloat("_GlowIntensity", (4 - currentHealthPercentage * 4) / 2);
    }

    public void TakeDamage(float damageAmount)
    {
        // ��������� ����� �����������
        Debug.Log("������� ���� � "+  damageAmount);
        float currentSpeed = _glowMat.GetFloat("_PulseSpeed");        
        _glowMat.SetFloat("_PulseSpeed", currentSpeed + damageAmount);
        float currentGlow = _glowMat.GetFloat("_GlowIntensity");
        _glowMat.SetFloat("_GlowIntensity", currentGlow + damageAmount);                
        // ��������� ������ ��������
        /*StartCoroutine(GlowEffect());*/
    }

    IEnumerator GlowEffect()
    {
        float elapsed = 0;
        while (elapsed < 1f)
        {
            _glowMat.SetFloat("_GlowIntensity", Mathf.Sin(elapsed * Mathf.PI) * 5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _glowMat.SetFloat("_GlowIntensity", 0);
    }
}