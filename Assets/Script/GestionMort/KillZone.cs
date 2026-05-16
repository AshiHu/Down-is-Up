using UnityEngine;
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // verificatiobn du tag a la collision
        if (other.CompareTag("Player"))
            // appel du script respawn et de la fonction Die
            other.GetComponent<Respawn>()?.Die();

        // appel du script respawnableObject et de la fonction Respawn
        RespawnableObject obj = other.GetComponent<RespawnableObject>();
        if (obj != null)
            obj.Respawn();
    }
    // systeme pour les collision physique et non trigger
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<Respawn>()?.Die();
    }
}