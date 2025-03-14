using UnityEngine;

public interface IBoss
{
    void InitBoss(Transform player);
    void ActivatePhase(int phaseNumber);
    void OnBossDeath();
}