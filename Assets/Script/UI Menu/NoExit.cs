using UnityEngine;

public class NoExit : MonoBehaviour
{
    public GameObject leFondA_Fermer;

    public void FermerLeFond()
    {
        if (leFondA_Fermer != null)
        {
            leFondA_Fermer.SetActive(false);

            Debug.Log("Le panneau a été fermé !");
        }
    }
}