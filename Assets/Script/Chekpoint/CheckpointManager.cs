using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // Utilisation de singleton car c'est un script manager et il n'y en qu'un. creer une instance statique pour y accéder facilement depuis d'autres scripts.
    public static CheckpointManager instance;

    // stockage des informations de chaque checkpoint 
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private Vector3 respawnGravity;
    private bool hasCheckpoint;

    // au debut au lencement on verifie si il n'y a pas de doublon du script si il y en a un on detruit le gameobject.
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

    // fonction que l'on appelle dans le script Checkpoint pour sauvegarder les informations du checkpoint actuel.
    public void SetCheckpoint(Vector3 position, Quaternion playerRotation, Vector3 gravityDirection)
    {
        respawnPosition  = position;
        respawnRotation  = playerRotation;
        respawnGravity   = gravityDirection;
        hasCheckpoint    = true;
    }
    // un itilisation de out pour avoir plusieurs valeurs de retour dans la fonction TryGetRespawnPoint qui permet de recuperer les informations du checkpoint actuel. elle retourne un booleen pour savoir si il y a un checkpoint ou pas.
    public bool TryGetRespawnPoint(out Vector3 position, out Quaternion rotation, out Vector3 gravity)
    {
        position = respawnPosition;
        rotation = respawnRotation;
        gravity  = respawnGravity;
        return hasCheckpoint;
    }
}
