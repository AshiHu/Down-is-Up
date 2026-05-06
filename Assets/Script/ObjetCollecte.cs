using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private void Start()
    {
        // S'enregistre auprès du manager pour le reset au respawn
        CollectibleManager.instance?.RegisterItem(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CollectibleManager.instance?.CollectItem();
        gameObject.SetActive(false);
    }

    // Remet l'objet en jeu
    public void ResetItem()
    {
        gameObject.SetActive(true);
    }
}