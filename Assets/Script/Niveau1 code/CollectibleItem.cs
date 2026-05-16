using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private void Start()
    {
        // Enregistre l'objet dans le CollectibleManager 
        CollectibleManager.instance?.RegisterItem(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CollectibleManager.instance?.CollectItem();
        gameObject.SetActive(false);
    }

    // Reset les items si on meurt 
    public void ResetItem()
    {
        gameObject.SetActive(true);
    }
}