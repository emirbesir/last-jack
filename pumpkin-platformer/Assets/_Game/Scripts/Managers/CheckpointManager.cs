using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{   
    private const float Y_OFFSET = 0.5f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform initialSpawnPoint;
    
    private Transform checkpoint;

    // Singleton
    public static CheckpointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        checkpoint = initialSpawnPoint;
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (checkpoint != null)
        {
            player.position = checkpoint.position + Vector3.up * Y_OFFSET;
        }
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }
}
