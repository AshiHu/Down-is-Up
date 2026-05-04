using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private Vector3 respawnGravity;
    private bool hasCheckpoint;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCheckpoint(Vector3 position, Quaternion playerRotation, Vector3 gravityDirection)
    {
        respawnPosition  = position;
        respawnRotation  = playerRotation;
        respawnGravity   = gravityDirection;
        hasCheckpoint    = true;
    }

    public bool TryGetRespawnPoint(out Vector3 position, out Quaternion rotation, out Vector3 gravity)
    {
        position = respawnPosition;
        rotation = respawnRotation;
        gravity  = respawnGravity;
        return hasCheckpoint;
    }
}
