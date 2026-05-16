using UnityEngine;

public class Arrow : MonoBehaviour
{
    // liste de mes fleches
    public GameObject[] arrow;

    // fonction qui active la fleche en fonction de l'index
    public void ArrowActive(int index)
    {
        foreach (GameObject f in arrow)
        {
            f.SetActive(false);
        }

        if (index >= 0 && index < arrow.Length)
        {
            arrow[index].SetActive(true);
        }
    }

    // fonction qui desactive toutes les fleches
    public void HideArrow()
    {
        foreach (GameObject f in arrow)
        {
            f.SetActive(false);
        }
    }
}