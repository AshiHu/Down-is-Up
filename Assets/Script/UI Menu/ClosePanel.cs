using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject closePanel;

    public void FermerLeFond()
    {
        if (closePanel != null)
        {
            closePanel.SetActive(false);

            Debug.Log("Le panneau a été fermé !");
        }
    }
}