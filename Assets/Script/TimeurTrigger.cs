using UnityEngine;

// TimerTrigger.cs — Lance le timer quand le joueur entre dans la zone
// À mettre sur un GameObject avec un Collider en Is Trigger

public class TimerTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        CollectibleManager.instance?.StartTimer();
    }
    public void Reset()
    {
        triggered = false;
    }
}