using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public static bool IsOpen = false;

    void Start()
    {
        // faux de base 
        IsOpen = false;
        settingsPanel.SetActive(false);
    
        Time.timeScale = 1f;

        // Verrouille le curseur au centre de l'écran et le rend invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Echap pour ouvrir/fermer le menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        IsOpen = !IsOpen;
        settingsPanel.SetActive(IsOpen);

        if (IsOpen)
        {
            // Met le temps à l'arrêt et affiche le curseur
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Remet le temps à la normale et cache le curseur
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}