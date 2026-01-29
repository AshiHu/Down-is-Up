using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public static bool IsOpen = false;

    void Start()
    {
        IsOpen = false;
        settingsPanel.SetActive(false);

    
        Time.timeScale = 1f;

     
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    // Cette fonction gère l'ouverture ET la fermeture
    // C'est elle qu'il faut mettre sur ton bouton "NON"
    public void ToggleMenu()
    {
        IsOpen = !IsOpen;
        settingsPanel.SetActive(IsOpen);

        if (IsOpen)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Cette fonction est pour ton bouton "QUITTER"
    public void QuitGame()
    {
        Debug.Log("Le jeu se ferme...");
        Application.Quit();
    }
}