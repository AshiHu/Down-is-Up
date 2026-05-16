using UnityEngine;
using UnityEngine.UI; 

public class GestionMenu : MonoBehaviour
{
    // liste des fonds
    public GameObject[] tousLesFonds; 

    public void SelectionnerBouton(int index)
    {
        GameObject boutonClique = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        OpenFond(index);
    }

    // activation du fond en fonction de l'index et desactive les autres
    private void OpenFond(int index)
    {
        foreach (GameObject f in tousLesFonds) f.SetActive(false);
        if (index < tousLesFonds.Length) tousLesFonds[index].SetActive(true);
    }
}