using UnityEngine;
using System.Collections;

public class GravityManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public S_Perso movement;

    // utilisation de hideInInspector pour ne pas afficher cette variable dans l'inspecteur
    [HideInInspector]
    public Vector3 gravityDirection = Vector3.down;
    public bool isTransitioning = false;
    private Coroutine rotationCoroutine;

    //fonction que l'on appel pour faire la transition de gravite
    public void UpdateGravityDirection(Vector3 newDirection)
    {
        gravityDirection = newDirection.normalized;
    }

    // On re met a zero la gravite pour quand on respawn
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