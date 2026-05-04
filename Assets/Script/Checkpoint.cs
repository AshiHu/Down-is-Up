using UnityEngine;

// Mettre ce script sur chaque checkpoint.
// Le collider du checkpoint doit avoir "Is Trigger" coché.
// Le joueur doit avoir le tag "Player".
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool debugLog = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        S_Perso movement = other.GetComponent<S_Perso>();
        Vector3 gravity = movement != null && movement.gravityManager != null
            ? movement.gravityManager.gravityDirection
            : Vector3.down;

        // On sauvegarde : position du checkpoint, rotation actuelle du joueur, gravité actuelle
        CheckpointManager.instance.SetCheckpoint(
            transform.position,
            other.transform.rotation,
            gravity
        );

        if (debugLog)
            Debug.Log($"Checkpoint activé : {gameObject.name}");
    }
}
