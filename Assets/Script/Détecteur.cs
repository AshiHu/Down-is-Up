using UnityEngine;
using UnityEngine.Events;

public class PressureSensor : MonoBehaviour
{
    [Header("Filtrage")]
    public string requiredTag = "oui";

    [Header("Événement")]
    public UnityEvent onTriggered;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!IsValidObject(other)) return;

        _triggered = true;
        onTriggered?.Invoke();
    }

    private bool IsValidObject(Collider other)
    {
        if (!string.IsNullOrEmpty(requiredTag))
            return other.CompareTag(requiredTag);

        return other.attachedRigidbody != null;
    }
}