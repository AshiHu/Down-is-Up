using UnityEngine;

public class Exit : MonoBehaviour
{
    public void QuitterLeJeu()
    {
        // bouton quitter le jeux 
        Debug.Log("Le bouton Quitter a été cliqué !");

        Application.Quit();

        //methode pour quitter le jeux dans l'editeur unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}