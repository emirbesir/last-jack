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
        if (!IsPrimaryInstance)
        {
            return;
        }

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
