using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float respawnDelaySeconds;

    private void OnEnable()
    {
        Flame.Instance.OnFlameDepleted += HandleFlameDepleted;
    }
    
    private void OnDisable()
    {
        Flame.Instance.OnFlameDepleted -= HandleFlameDepleted;
    }

    private void HandleFlameDepleted()
    {
        StartCoroutine(RespawnPlayerAfterDelay(respawnDelaySeconds));
    }

    IEnumerator RespawnPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CheckpointManager.Instance.RespawnPlayer();
        ScreenManager.Instance.ScreenOpeningEffect();
        Flame.Instance.ResetFlame();
    }
}
