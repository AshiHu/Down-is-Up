using UnityEngine;
using UnityEngine.SceneManagement;

public class MapChange : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string sceneName;

    // lance la scene specifiee dans l'inspector avec le nom exact de la scene voulue
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