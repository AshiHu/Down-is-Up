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


    // utilisation d'un singleton car c'est un game manager 
    void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // on met les ui a false pour pas les faire apparaitre avant le début du timer, et on initialise le timer
    void Start()
    {
        timeRemaining = timerDuration;
        timerRunning = false;
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (collectibleText != null) collectibleText.gameObject.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        // gestion du timer
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

    public void StartTimer()
    {
        // activation des ui et lancement du timer
        timerRunning = true;
        if (timerText != null) timerText.gameObject.SetActive(true);
        if (collectibleText != null) collectibleText.gameObject.SetActive(true);
    }

    // s'ajoute dans la liste des collectibles pour pouvoir les reset lors de la mort du joueur
    public void RegisterItem(CollectibleItem item)
    {
        if (!allItems.Contains(item))
            allItems.Add(item);
    }
    // augmente le nombre de 1 et vérifie si le nombre requis est atteint pour ouvrir la porte
    public void CollectItem()
    {
        currentCount++;
        UpdateUI();
        if (currentCount >= requiredCount)
            OpenDoor();
    }
    // on regarde si on a atteint le nombre requis de collectibles, si oui on ouvre la porte, sinon on active la killzone
    private void OnTimerEnd()
    {
        if (currentCount >= requiredCount)
            OpenDoor();
        else
            killZone?.SetActive(true);
    }

    // ouvre la porte et désactive les ui
    private void OpenDoor()
    {
        if (door != null)
            door.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (collectibleText != null) collectibleText.gameObject.SetActive(false);
    }

    // reset le niveau : désactive la killzone, reset les collectibles, reset le timer et les ui
    public void OnPlayerRespawn()
    {
        killZone?.SetActive(false);
        currentCount = 0;

        allItems.RemoveAll(item => item == null);
        foreach (CollectibleItem item in allItems)
            item.ResetItem();

        timeRemaining = timerDuration;
        timerRunning = false;
        timerTrigger?.Reset();
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (collectibleText != null) collectibleText.gameObject.SetActive(false);
        UpdateUI();
    }

    // mis en place des ui pour afficher le temps restant et le nombre de collectibles collectes
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