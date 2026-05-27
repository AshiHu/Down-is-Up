using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Particle Système")]
    public ParticleSystem Emit;

    [SerializeField] private bool debugLog = true;

    void OnTriggerEnter(Collider other)
    {
        // regarde si le joueur a bien comme tag player 
        if (!other.CompareTag("Player")) return;

        //Play particule
        if (Emit != null)
            Emit.Play();
    
        // récupère le composant S_Perso du joueur pour obtenir la direction de gravité actuelle et on regarde si il a un gravityManager
        S_Perso movement = other.GetComponent<S_Perso>();
        Vector3 gravity = movement != null && movement.gravityManager != null
            ? movement.gravityManager.gravityDirection
            : Vector3.down;

        // sauvegarde du tchekpoint dans le CheckpointManager
        CheckpointManager.instance.SetCheckpoint(
            transform.position,
            other.transform.rotation,
            gravity
        );

        if (debugLog)
            Debug.Log($"Checkpoint activé : {gameObject.name}");
    }
}
