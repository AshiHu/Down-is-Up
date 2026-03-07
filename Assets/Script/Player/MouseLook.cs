using UnityEngine;

// MouseLook.cs — Contrôle de la caméra à la souris
// Ce script gère la rotation de la caméra selon les mouvements
// de la souris, avec lissage (smooth) pour éviter les mouvements
// brusques.
// Fonctionnement :
//   - Mouvement horizontal souris → rotation du corps du joueur (axe Y)
//   - Mouvement vertical souris   → rotation de la caméra (axe X, clampée à ±90°)
//   - Pendant une transition de gravité → les inputs souris sont bloqués
//     pour éviter un conflit avec la rotation de GravityManager

public class MouseLook : MonoBehaviour
{
    // RÉFÉRENCES PLAYER
    public Transform playerBody;         // Transform du corps du joueur (pour la rotation horizontale)
    public Transform cameraTransform;    // Transform de la caméra (pour la rotation verticale)
    public float mouseSensitivity = 2f;  // Multiplicateur de la vitesse de rotation

    public GravityManager gravityManager;
    // Référence au GravityManager pour détecter si une transition est en cours

    // LISSAGE DU MOUVEMENT 
    [Header("Parametres de Smooth")]
    public float smoothSpeed = 15f;
    // Vitesse de lissage des mouvements de caméra.
    // Valeur haute = réactif (peu de lissage).
    // Valeur basse = mouvement très fluide/lent à répondre.

    // VARIABLES INTERNES
    private float xRotation = 0f;
    // Angle de rotation verticale accumulé (inclinaison haut/bas de la caméra)
    // Clampé entre -90° et +90° pour éviter de "retourner" la caméra

    private float smoothX = 0f; // Valeur lissée du mouvement horizontal souris
    private float smoothY = 0f; // Valeur lissée du mouvement vertical souris


    void Start()
    {
        // On verrouille le curseur au centre de l'écran (mode FPS classique)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        // Auto-assign du GravityManager si pas défini dans l'inspecteur
        if (gravityManager == null)
            gravityManager = FindFirstObjectByType<GravityManager>();
    }

    void Update()
    {
        // Si le menu des paramètres est ouvert, on bloque la rotation caméra
        if (SettingsManager.IsOpen) return;

        // Pendant une transition de gravité, on bloque complètement la souris
        // pour éviter que le joueur interfère avec la rotation automatique du GravityManager
        float inputMultiplier = (gravityManager != null && gravityManager.isTransitioning) ? 0f : 1f;

        // Lecture des axes souris, multipliés par la sensibilité et le multiplicateur de blocage
        float targetMouseX = Input.GetAxis("Mouse X") * mouseSensitivity * inputMultiplier;
        float targetMouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * inputMultiplier;

        // Lissage : on interpole vers la valeur cible pour éviter les mouvements saccadés
        smoothX = Mathf.Lerp(smoothX, targetMouseX, Time.deltaTime * smoothSpeed);
        smoothY = Mathf.Lerp(smoothY, targetMouseY, Time.deltaTime * smoothSpeed);

        // ROTATION HORIZONTALE
        // On fait tourner le corps du joueur sur son axe Y local (gauche/droite)
        playerBody.Rotate(Vector3.up * smoothX);

        // ROTATION VERTICALE
        // On soustrait le mouvement vertical (souris vers le haut = caméra vers le haut = xRotation diminue)
        xRotation -= smoothY;

        // Clamp pour empêcher la caméra de dépasser 90° (regarder derrière en montant/descendant)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // On applique la rotation verticale uniquement à la caméra (pas au corps)
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
