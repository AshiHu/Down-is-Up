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

        // point aleatoire si aucun checkpoint n'est defini
        if (spawnPoints != null && spawnPoints.Length > 0)
            initialSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
    }

    public void Die()
    {
        // securite pour pas mourir plusieurs fois d'affile
        if (isDead) return;
        isDead = true;

        // on recupere le tcheckpoint manager pour savoir ou respawn
        if (CheckpointManager.instance != null &&
            CheckpointManager.instance.TryGetRespawnPoint(out Vector3 pos, out Quaternion rot, out Vector3 gravity))
        {
            DoRespawn(pos, rot, gravity);
        }
        // sinon on respawn au point de spawn initial s'il existe
        else if (initialSpawnPoint != null)
        {
            DoRespawn(initialSpawnPoint.position, initialSpawnPoint.rotation, Vector3.down);
        }

        // sinon on respawn a la position d'origine 0,0,0 avec gravite vers le bas
        else
        {
            DoRespawn(Vector3.zero, Quaternion.identity, Vector3.down);
        }
    }

    private void DoRespawn(Vector3 position, Quaternion rotation, Vector3 gravity)
    {
        // on recupere la gravite dans le checkpoint manager ppour l'appliquer au respawn
        if (movement != null && movement.gravityManager != null)
            movement.gravityManager.ResetGravity(gravity);

        // deactivation et reactivation du character controller pour eviter les problemes de collision lors du repositionnement
        if (cc != null) cc.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        if (cc != null) cc.enabled = true;

        // vitesse du perso a 0
        if (movement != null)
            movement.ResetMovement();

        // Previens que le joueur et mort dans le collectible manager pour reset les collectibles si besoin
        CollectibleManager.instance?.OnPlayerRespawn();

        isDead = false;
    }
}
