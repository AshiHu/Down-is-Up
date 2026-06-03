using UnityEngine;

public class OpenNiveauDeuxFIn : MonoBehaviour
{
    public GameObject door;

    private void Start()
    {
        door.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("oui"))
        {
            door.SetActive(false);
        }
    }
}
