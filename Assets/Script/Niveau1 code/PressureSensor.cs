using UnityEngine;
using UnityEngine.Events;

public class PressureSensor : MonoBehaviour
{
    [Header("Filtrage")]
    public string requiredTag = "oui";

    [Header("Événement")]
    public UnityEvent onTriggered;

    private bool _triggered = false;

    // on trigger le gameObject qui a le tag oui pour ensuite déclencher l'evenement onTriggered
    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;

        // lance l'evenement qui peut etre assigné dans l'inspecteur de unity en occurence dans la scene 1 pour ouvrir la porte dans le door script, fonction open
        _triggered = true;
        onTriggered?.Invoke();
    }
}