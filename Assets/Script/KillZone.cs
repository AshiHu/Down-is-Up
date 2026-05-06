using UnityEngine;
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Joueur
        if (other.CompareTag("Player"))
            other.GetComponent<Respawn>()?.Die();
        // Objet lançable
        RespawnableObject obj = other.GetComponent<RespawnableObject>();
        if (obj != null)
            obj.Respawn();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<Respawn>()?.Die();
    }
}