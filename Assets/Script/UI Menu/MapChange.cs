using UnityEngine;
using UnityEngine.SceneManagement;

public class MapChange : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string sceneName; // Le nom exact de ta scène

    // Cette fonction sera appelée par le bouton
    public void LaunchScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Oups ! Tu as oublié de donner le nom de la scène dans l'Inspector.");
        }
    }
}