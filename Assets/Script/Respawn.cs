using UnityEngine;

public class Respawn : MonoBehaviour
{
    [Header("Spawn Points de départ (avant le 1er checkpoint)")]
    [SerializeField] private GameObject[] spawnPoints;

    private Transform initialSpawnPoint;
    private bool isDead;
    private CharacterController cc;
    private S_Perso movement;

    void Start()
    {
        isDead   = false;
        cc       = GetComponent<CharacterController>();
        movement = GetComponent<S_Perso>();

        if (spawnPoints != null && spawnPoints.Length > 0)
            initialSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (CheckpointManager.instance != null &&
            CheckpointManager.instance.TryGetRespawnPoint(out Vector3 pos, out Quaternion rot, out Vector3 gravity))
        {
            DoRespawn(pos, rot, gravity);
        }
        else if (initialSpawnPoint != null)
        {
            DoRespawn(initialSpawnPoint.position, initialSpawnPoint.rotation, Vector3.down);
        }
        else
        {
            DoRespawn(Vector3.zero, Quaternion.identity, Vector3.down);
        }
    }

    private void DoRespawn(Vector3 position, Quaternion rotation, Vector3 gravity)
    {
        // Restaure la gravité immédiatement, sans transition animée
        if (movement != null && movement.gravityManager != null)
            movement.gravityManager.ResetGravity(gravity);

        // CharacterController bloque le déplacement direct de transform, on le désactive le temps de téléporter
        if (cc != null) cc.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        if (cc != null) cc.enabled = true;

        if (movement != null)
            movement.ResetMovement();
        
        // Notifie le CollectibleManager du respawn
        CollectibleManager.instance?.OnPlayerRespawn();

        isDead = false;
    }
}
