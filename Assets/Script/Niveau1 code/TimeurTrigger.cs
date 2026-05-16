using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        // StartTimer dans le CollectibleManager
        triggered = true;
        CollectibleManager.instance?.StartTimer();
    }
    public void Reset()
    {
        triggered = false;
    }
}