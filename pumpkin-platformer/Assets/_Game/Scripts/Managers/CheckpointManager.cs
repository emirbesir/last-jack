using UnityEngine;

public class CheckpointManager : SingletonMonoBehaviour<CheckpointManager>
{   
    private const float Y_OFFSET = 0.5f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform initialSpawnPoint;
    
    private Transform checkpoint;

    protected override void Awake()
    {
        base.Awake();

        checkpoint = initialSpawnPoint;
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        player.position = checkpoint.position + Vector3.up * Y_OFFSET;
        Debug.Log("Player respawned at checkpoint: " + checkpoint.position);
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }
}
