using UnityEngine;
using System.Collections;

// GravityManager.cs — Gestionnaire de gravité multi-direction
// La gravité est contrôlée uniquement par contact avec les surfaces (layer SOL)
// via WalkableSurfaceGravity qui appelle UpdateGravityDirection().

public class GravityManager : MonoBehaviour
{
    // RÉFÉRENCES PLAYER
    [Header("References")]
    public Transform player;
    public S_Perso movement;

    // ÉTAT DE LA GRAVITÉ
    [HideInInspector]
    public Vector3 gravityDirection = Vector3.down;

    public bool isTransitioning = false;

    private Coroutine rotationCoroutine;

    // MISE À JOUR EXTERNE DE LA DIRECTION DE GRAVITÉ
    // Appelé par WalkableSurfaceGravity lors d'un contact de surface.
    public void UpdateGravityDirection(Vector3 newDirection)
    {
        gravityDirection = newDirection.normalized;
    }

    // RESET IMMÉDIAT — utilisé au respawn pour restaurer la gravité sans transition animée
    public void ResetGravity(Vector3 direction)
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
        isTransitioning = false;
        gravityDirection = direction.normalized;
    }
}