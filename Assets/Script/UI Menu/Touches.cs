using UnityEngine;
using TMPro;
using System;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager instance;

    [Header("Touches")]
    public KeyCode jumpKey;
    public KeyCode slideKey;

    [Header("UI Textes")]
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI slideText;

    private string waitingForKey = "";

    void Awake()
    {
        if (instance == null) instance = this;

        // On charge les touches sauvegardées ou on met des touches par défaut
        jumpKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JumpKey", "Space"));
        slideKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("SlideKey", "LeftControl"));

        UpdateUI();
    }

    void OnGUI() // Détecte l'appui sur le clavier
    {
        if (waitingForKey != "")
        {
            Event e = Event.current;
            if (e.isKey && e.keyCode != KeyCode.None)
            {
                SetNewKey(waitingForKey, e.keyCode);
                waitingForKey = "";
            }
        }
    }

    public void StartAssignment(string keyName)
    {
        waitingForKey = keyName;
        // On change le texte pour dire qu'on attend
        if (keyName == "Jump") jumpText.text = "...";
        if (keyName == "Slide") slideText.text = "...";
    }

    void SetNewKey(string keyName, KeyCode clickedKey)
    {
        if (keyName == "Jump") jumpKey = clickedKey;
        if (keyName == "Slide") slideKey = clickedKey;

        PlayerPrefs.SetString(keyName + "Key", clickedKey.ToString());
        UpdateUI();
    }

    void UpdateUI()
    {
        if (jumpText) jumpText.text = jumpKey.ToString();
        if (slideText) slideText.text = slideKey.ToString();
    }
}