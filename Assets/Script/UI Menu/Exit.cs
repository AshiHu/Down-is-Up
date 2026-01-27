using UnityEngine;

public class Exit : MonoBehaviour
{
    public void QuitterLeJeu()
    {
        Debug.Log("Le bouton Quitter a été cliqué !");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}