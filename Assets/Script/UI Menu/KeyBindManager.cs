using UnityEngine;
using TMPro;
using System;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager instance;

    [Header("Touches")]
    public KeyCode jumpKey;
    public KeyCode slideKey;
    public KeyCode gravityRotateKey;

    [Header("UI Textes")]
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI slideText;
    public TextMeshProUGUI gravityRotateText;

    private string waitingForKey = "";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        jumpKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JumpKey", "Space"));
        slideKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("SlideKey", "LeftControl"));
        gravityRotateKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("GravityRotateKey", "E"));

        UpdateUI();
    }
    void OnGUI()
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
        if (keyName == "Jump") jumpText.text = "...";
        if (keyName == "Slide") slideText.text = "...";
        if (keyName == "GravityRotate" && gravityRotateText) gravityRotateText.text = "...";
    }

    void SetNewKey(string keyName, KeyCode clickedKey)
    {
        if (keyName == "Jump") jumpKey = clickedKey;
        if (keyName == "Slide") slideKey = clickedKey;
        if (keyName == "GravityRotate") gravityRotateKey = clickedKey;

        PlayerPrefs.SetString(keyName + "Key", clickedKey.ToString());
        UpdateUI();
    }

    void UpdateUI()
    {
        if (jumpText) jumpText.text = jumpKey.ToString();
        if (slideText) slideText.text = slideKey.ToString();
        if (gravityRotateText) gravityRotateText.text = gravityRotateKey.ToString();
    }
}