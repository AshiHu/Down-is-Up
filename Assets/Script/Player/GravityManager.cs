using UnityEngine;
using System.Collections;

// ============================================================
// GravityManager.cs — Gestionnaire de gravité multi-direction
// ============================================================
// Ce script gère la direction de la gravité dans le jeu et
// fait pivoter le joueur en douceur quand la gravité change.
//
// Fonctionnement :
//   - Le joueur peut changer la gravité en regardant dans une
//     direction et en appuyant sur la touche dédiée (dans S_Perso)
//   - La gravité s'aligne sur l'une des 6 directions cardinales
//     (bas, haut, gauche, droite, avant, arrière)
//   - Une coroutine gère la transition de rotation pour la rendre fluide
// ============================================================

public class GravityManager : MonoBehaviour
{
    // -------------------------
    // RÉFÉRENCES
    // -------------------------
    [Header("References")]
    public Transform player;                   // Transform du GameObject joueur
    public S_Perso movement;                   // Référence au script de déplacement du joueur
    public Camera playerCamera;                // Caméra principale (pour détecter la direction du regard)

    // -------------------------
    // PARAMÈTRES DE TRANSITION
    // -------------------------
    [Header("Parametres Transition")]
    public float transitionDuration = 0.7f;
    // Durée (en secondes) de la rotation du joueur lors d'un changement de gravité

    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    // Courbe d'animation qui contrôle l'interpolation de la rotation
    // EaseInOut = démarre et finit lentement, accélère au milieu → rendu naturel

    // -------------------------
    // ÉTAT DE LA GRAVITÉ
    // -------------------------
    [HideInInspector]
    public Vector3 gravityDirection = Vector3.down;
    // Direction actuelle de la gravité. Utilisée par S_Perso et PickUp.
    // [HideInInspector] = visible en code mais pas dans l'inspecteur Unity

    public bool isTransitioning = false;
    // true pendant la durée de la rotation → bloque les inputs souris (MouseLook)
    // et empêche un second changement de gravité avant la fin

    private Coroutine rotationCoroutine;
    // Référence à la coroutine de rotation en cours (pour pouvoir l'annuler si besoin)

    // -------------------------
    // DIRECTIONS CARDINALES
    // -------------------------
    // Les 6 directions possibles pour la gravité
    // Le joueur regarde vers la plus proche pour déterminer la nouvelle gravité
    private readonly Vector3[] cardinalDirections = new Vector3[]
    {
        Vector3.down,    // Gravité normale (plancher)
        Vector3.up,      // Gravité inversée (plafond)
        Vector3.left,    // Mur gauche
        Vector3.right,   // Mur droit
        Vector3.forward, // Mur avant
        Vector3.back     // Mur arrière
    };

    // -------------------------
    // INITIALISATION
    // -------------------------
    void Start()
    {
        // Si la caméra n'est pas assignée manuellement, on prend la caméra principale
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    // -------------------------
    // DÉCLENCHEMENT DU CHANGEMENT DE GRAVITÉ
    // -------------------------
    // Appelé depuis S_Perso quand le joueur appuie sur la touche de rotation de gravité.
    // Détermine la nouvelle direction de gravité selon où regarde la caméra.
    public void TriggerGravityFromCameraLook()
    {
        // Direction dans laquelle la caméra regarde
        Vector3 cameraForward = playerCamera.transform.forward;

        Vector3 bestDirection = Vector3.down;
        float bestDot = -Mathf.Infinity;

        // On cherche la direction cardinale la plus alignée avec le regard de la caméra
        // (produit scalaire le plus élevé = angle le plus faible = meilleur alignement)
        foreach (Vector3 dir in cardinalDirections)
        {
            float dot = Vector3.Dot(cameraForward, dir);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestDirection = dir;
            }
        }

        // Si la meilleure direction est déjà la gravité actuelle, on ne fait rien
        if (bestDirection == gravityDirection) return;

        // Sinon, on lance la transition
        StartGravityTransition(bestDirection);
    }

    // -------------------------
    // DÉMARRAGE DE LA TRANSITION
    // -------------------------
    void StartGravityTransition(Vector3 newGravity)
    {
        // On met à jour immédiatement la direction de gravité
        // (les scripts comme S_Perso et PickUp l'utiliseront dès la prochaine frame)
        gravityDirection = newGravity.normalized;

        // On remet la vélocité du joueur à zéro pour éviter une propulsion parasite
        if (movement != null) movement.velocity = Vector3.zero;

        // Si une rotation est déjà en cours, on l'arrête avant d'en lancer une nouvelle
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);

        // On lance la coroutine de rotation fluide
        rotationCoroutine = StartCoroutine(SmoothRotationRoutine(newGravity));
    }

    // -------------------------
    // COROUTINE : ROTATION FLUIDE DU JOUEUR
    // -------------------------
    IEnumerator SmoothRotationRoutine(Vector3 newGravity)
    {
        isTransitioning = true;

        // Le "haut" du joueur dans la nouvelle gravité = opposé de la direction de gravité
        Vector3 targetUp = -newGravity.normalized;

        // Rotation de départ (la rotation actuelle du joueur)
        Quaternion startRotation = player.rotation;

        // On calcule l'axe "avant" du joueur projeté sur le nouveau plan horizontal
        // pour que le joueur reste orienté dans la même direction après la rotation
        Vector3 forward = Vector3.ProjectOnPlane(player.forward, targetUp);

        // Cas limite : si le forward est trop parallèle au targetUp, on prend le right à la place
        if (forward.sqrMagnitude < 0.01f)
            forward = Vector3.ProjectOnPlane(player.right, targetUp);

        // Rotation cible : le joueur "debout" selon la nouvelle gravité, orienté vers forward
        Quaternion targetRotation = Quaternion.LookRotation(forward, targetUp);

        float elapsed = 0f;

        // On interpole progressivement entre la rotation initiale et la cible
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime; // unscaledDeltaTime = fonctionne même si Time.timeScale = 0

            // On évalue la courbe d'animation pour un mouvement non-linéaire (EaseInOut)
            float t = rotationCurve.Evaluate(Mathf.Clamp01(elapsed / transitionDuration));

            // Slerp = interpolation sphérique entre deux rotations (plus naturel qu'un Lerp)
            player.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null; // On attend la prochaine frame
        }

        // On force la rotation finale exacte (évite les micro-décalages)
        player.rotation = targetRotation;

        isTransitioning = false;
    }
}
