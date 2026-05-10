using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager instance;

    [Header("Collectibles")]
    public int requiredCount = 5;
    private int currentCount = 0;
    private List<CollectibleItem> allItems = new List<CollectibleItem>();

    [Header("Porte")]
    public GameObject door;

    [Header("Timer")]
    public float timerDuration = 60f;
    private float timeRemaining;
    private bool timerRunning = false;

    [Header("KillZone")]
    public GameObject killZone; 

    [Header("UI (optionnel)")]
    public Text timerText;
    public Text collectibleText;

    [Header("Timer Trigger")]
    public TimerTrigger timerTrigger;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timeRemaining = timerDuration;
        timerRunning = false;
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (collectibleText != null) collectibleText.gameObject.SetActive(false); // ajout
        UpdateUI();
    }

    void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;
        UpdateUI();

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;
            OnTimerEnd();
        }
    }

    // Appelé par TimerTrigger
    public void StartTimer()
    {
        timerRunning = true;
        if (timerText != null) timerText.gameObject.SetActive(true);
        if (collectibleText != null) collectibleText.gameObject.SetActive(true);
    }

    // Appelé par CollectibleItem au Start() pour s'enregistrer
    public void RegisterItem(CollectibleItem item)
    {
        if (!allItems.Contains(item))
            allItems.Add(item);
    }

    public void CollectItem()
    {
        currentCount++;
        UpdateUI();

        if (currentCount >= requiredCount)
            OpenDoor();
    }

    private void OnTimerEnd()
    {
        if (currentCount >= requiredCount)
            OpenDoor();
        else
            killZone?.SetActive(true); // On active simplement le GameObject
    }

    private void OpenDoor()
    {
        if (door != null)
            door.SetActive(false);

        if (timerText != null) timerText.gameObject.SetActive(false);
        if (collectibleText != null) collectibleText.gameObject.SetActive(false);
    }

    // Appelé par Respawn.cs après un Die()
    public void OnPlayerRespawn()
    {
        killZone?.SetActive(false);
        currentCount = 0;
        foreach (CollectibleItem item in allItems)
            item.ResetItem();
        timeRemaining = timerDuration;
        timerRunning = false;
        timerTrigger?.Reset(); // reset du trigger
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = $"Temps : {seconds}s";
        }

        if (collectibleText != null)
            collectibleText.text = $"{currentCount} / {requiredCount}";
    }
}