using UnityEngine;

// ============================================================
// CameraFOV.cs — Gestion dynamique du champ de vision (FOV)
// ============================================================
// Ce script fait varier le FOV (Field of View) de la caméra
// en fonction de l'état du joueur, pour renforcer les sensations
// de vitesse et de mouvement.
//
// Comportement :
//   - Au repos              → FOV de base (baseFOV)
//   - En mouvement          → baseFOV + 10°
//   - En glissade (slide)   → FOV maximum (maxFOV)
//
// La transition entre les valeurs est lissée avec un Lerp.
// ============================================================

public class CameraFOV : MonoBehaviour
{
    // -------------------------
    // RÉFÉRENCES
    // -------------------------
    [Header("References")]
    public S_Perso playerScript;
    // Référence au script de déplacement du joueur (S_Perso.cs)
    // Utilisé pour lire les axes de déplacement

    private Camera cam;
    // Référence au composant Camera Unity de ce GameObject

    // -------------------------
    // PARAMÈTRES DU FOV
    // -------------------------
    [Header("Parametres FOV")]
    public float baseFOV = 60f;
    // FOV par défaut quand le joueur est immobile

    public float maxFOV = 90f;
    // FOV maximal utilisé pendant la glissade

    // -------------------------
    // VITESSE DE TRANSITION
    // -------------------------
    [Header("Vitesse de transition")]
    [Tooltip("Plus la valeur est petite, plus c'est lent")]
    public float smoothSpeed = 2f;
    // Contrôle la fluidité de la transition du FOV.
    // Valeur recommandée : entre 1 (très lent) et 10 (très rapide).

    // -------------------------
    // INITIALISATION
    // -------------------------
    void Start()
    {
        // Récupère le composant Camera sur ce même GameObject
        cam = GetComponent<Camera>();

        // Auto-assign du script joueur si pas défini dans l'inspecteur
        if (playerScript == null)
            playerScript = FindFirstObjectByType<S_Perso>();
    }

    // -------------------------
    // BOUCLE PRINCIPALE
    // -------------------------
    void Update()
    {
        // Sécurité : si le script joueur n'est pas trouvé, on ne fait rien
        if (playerScript == null) return;

        // On part du FOV de base comme valeur cible par défaut
        float targetFOV = baseFOV;

        // On vérifie si le joueur appuie sur une touche de déplacement
        bool isMoving = Input.GetAxisRaw(playerScript.horizontalAxis) != 0 ||
                        Input.GetAxisRaw(playerScript.verticalAxis)   != 0;

        // En mouvement → légère augmentation du FOV pour une sensation de vitesse
        if (isMoving) targetFOV = baseFOV + 10f;

        // En glissade → FOV maximal pour accentuer la vitesse ressentie
        if (KeyBindManager.instance != null && Input.GetKey(KeyBindManager.instance.slideKey))
            targetFOV = maxFOV;

        // Interpolation fluide vers le FOV cible (évite les sauts brusques)
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
    }
}
