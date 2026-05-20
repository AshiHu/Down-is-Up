using UnityEngine;

public class ResetGame : MonoBehaviour
{
    void Awake()
    {
        if (PlayerPrefs.GetInt("GameInitialized", 0) == 0)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("GameInitialized", 1);
            PlayerPrefs.Save();
            Debug.Log("Premier lancement, reset effectué !");
        }
        else
        {
            Debug.Log("Déjà initialisé, pas de reset.");
        }
    }
}