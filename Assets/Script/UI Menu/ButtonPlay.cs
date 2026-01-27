using UnityEngine;

public class Boutonplay : MonoBehaviour
{
    // Glisse ton objet "Font" (celui qui a les 3 boutons en enfant) ici
    public GameObject leFondAShow;

    public void OpenSaveMenu()
    {
        if (leFondAShow != null)
        {
            // On affiche le fond (et donc tous ses enfants d'un coup !)
            leFondAShow.SetActive(true);
        }
        else
        {
            Debug.LogError("Oups ! Tu as oublié de glisser le 'Font' dans l'inspecteur.");
        }
    }
}