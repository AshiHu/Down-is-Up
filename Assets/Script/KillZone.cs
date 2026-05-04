using UnityEngine;

// Mettre ce script sur tout objet qui tue le joueur.
// Option A — Trigger invisible : cocher "Is Trigger" sur le collider, placer ce script.
// Option B — Collision solide : ajouter un collider enfant avec "Is Trigger" coché + ce script dessus.
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<Respawn>()?.Die();
    }

    // Garde aussi la collision directe au cas où le collider n'est pas un trigger
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        collision.gameObject.GetComponent<Respawn>()?.Die();
    }
}
