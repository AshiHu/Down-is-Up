using UnityEngine;
using System.Collections;

// S_Perso.cs — Gestion du déplacement du joueur
// Ce script gère tout ce qui touche au mouvement du personnage :
//   - Déplacement au clavier (ZQSD ou autre selon KeyBindManager)
//   - Saut
//   - Glissade (slide)
//   - Momentum (conservation de la vitesse)
//   - Gravité personnalisée (multi-direction via GravityManager)
//   - Réalignement du corps selon la direction de gravité

[RequireComponent(typeof(CharacterController))]
public class S_Perso : MonoBehaviour
{
    // PHYSIQUE DE BASE
    [Header("Physique")]
    public float moveSpeed = 8f;          // Vitesse de déplacement normale
    public float jumpHeight = 2.2f;       // Hauteur maximale du saut
    public float gravityValue = -20f;     // Force de gravité appliquée (négative = vers le bas)

    // MOMENTUM
    [Header("Momentum")]
    public float momentumDecay = 6f;
    // Vitesse à laquelle le momentum s'arrête ou s'applique.
    // Valeur haute = accélération/décélération rapide.
    // Valeur basse = impression de "glisse" ou d'inertie.

    // GLISSADE (SLIDE)
    [Header("Glissade")]
    public float slideSpeed = 15f;        // Vitesse initiale de la glissade
    public float slideDuration = 0.8f;    // Durée maximale de la glissade en secondes
    public float slideDecay = 4f;         // Vitesse de décélération pendant la glissade

    [HideInInspector] public bool isSliding = false;    // Est-ce que le joueur glisse en ce moment ?
    private Vector3 slideDirection;                      // Direction dans laquelle on glisse
    private float currentSlideSpeed;                     // Vitesse actuelle de glissade (diminue progressivement)
    private Coroutine slideCoroutine;                    // Référence à la coroutine de glissade (pour pouvoir l'annuler)

    // AXES DE CONTRÔLE
    [Header("Configuration des Axes")]
    public string verticalAxis = "Vertical";      // Axe avant/arrière (défini dans Input Manager Unity)
    public string horizontalAxis = "Horizontal";  // Axe gauche/droite

    // DÉTECTION DU SOL
    [Header("Détection Sol")]
    public float groundCheckDistance = 0.2f;  // Distance du raycast vers le bas pour détecter le sol
    public LayerMask groundLayer;             // Calques Unity considérés comme "sol"

    // RÉFÉRENCES GRAVITY MANAGER
    [Header("Références")]
    public GravityManager gravityManager;     // Référence au gestionnaire de gravité (GravityManager.cs)

    // VARIABLES PRIVÉES INTERNES
    private CharacterController controller;         // Composant CharacterController Unity (collision + mouvement)
    [HideInInspector] public Vector3 velocity;      // Vélocité verticale (gravité + saut)
    private Vector3 horizontalMomentum;             // Vélocité horizontale avec inertie
    private bool isGrounded;                        // Est-ce que le joueur touche le sol ?
    private float jumpTimer = 0f;
    // Petit délai après le saut pour éviter que isGrounded redevienne true immédiatement

    // INITIALISATION
    void Start()
    {
        // Récupère le CharacterController attaché à ce même GameObject
        controller = GetComponent<CharacterController>();
    }

    // BOUCLE PRINCIPALE
    void Update()
    {
        // Sécurité : si le GravityManager n'est pas assigné, on ne fait rien
        if (gravityManager == null) return;

        // Direction actuelle de la gravité (ex: Vector3.down en conditions normales)
        Vector3 gravityDir = gravityManager.gravityDirection.normalized;

        // "Haut" du joueur = opposé de la gravité
        Vector3 playerUp = -gravityDir;

        // RÉALIGNEMENT DU CORPS
        // Quand la gravité change de direction, on fait pivoter le joueur
        // pour que son "up" corresponde à la nouvelle gravité
        if (transform.up != playerUp)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, playerUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // DÉTECTION DU SOL
        // On utilise un Raycast vers le bas (selon la gravité) pour savoir si on touche le sol
        if (jumpTimer <= 0)
        {
            float rayLength = (controller.height / 2f) + groundCheckDistance;
            isGrounded = Physics.Raycast(transform.position, gravityDir, rayLength, groundLayer);

            // Debug visuel dans la Scene view : vert = au sol, rouge = en l'air
            Debug.DrawRay(transform.position, gravityDir * rayLength, isGrounded ? Color.green : Color.red);
        }
        else
        {
            // Pendant le jumpTimer, on considère qu'on n'est pas au sol
            // (évite le bug où on redetecte le sol juste après avoir sauté)
            isGrounded = false;
            jumpTimer -= Time.deltaTime;
        }

        // GESTION DE LA VÉLOCITÉ VERTICALE (GRAVITÉ)
        if (isGrounded)
        {
            // Si on est au sol et qu'on tombait vers le bas, on remet une vélocité légère
            // pour coller au sol (évite les rebonds ou la détection instable)
            float dot = Vector3.Dot(velocity, gravityDir);
            if (dot > 0) velocity = gravityDir * 2f;
        }
        else
        {
            // En l'air : on accumule la gravité sur la vélocité verticale
            velocity += gravityDir * -gravityValue * Time.deltaTime;
        }

        // LECTURE DES INPUTS
        float inputX = Input.GetAxisRaw(horizontalAxis); // -1, 0, ou 1
        float inputZ = Input.GetAxisRaw(verticalAxis);   // -1, 0, ou 1

        // On projette les directions locales du joueur sur le plan perpendiculaire à la gravité
        // Cela permet de marcher "à plat" quelle que soit l'orientation de la gravité
        Vector3 moveForward = Vector3.ProjectOnPlane(transform.forward, gravityDir).normalized;
        Vector3 moveRight   = Vector3.ProjectOnPlane(transform.right,   gravityDir).normalized;

        // Direction de mouvement combinée et normalisée
        Vector3 move = (moveRight * inputX + moveForward * inputZ).normalized;
        bool hasInput = move.sqrMagnitude > 0.1f; // true si une touche de direction est pressée

        // LOGIQUE DE DÉPLACEMENT
        if (isSliding)
        {
            // EN MODE GLISSADE
            // La vitesse de glissade diminue progressivement
            currentSlideSpeed = Mathf.Lerp(currentSlideSpeed, 0f, Time.deltaTime * slideDecay);
            controller.Move(slideDirection * currentSlideSpeed * Time.deltaTime);

            // On met à jour le momentum pour le conserver si on annule la glissade
            horizontalMomentum = slideDirection * currentSlideSpeed;

            // Annulation de la glissade si on rappuie sur la touche de slide
            if (KeyBindManager.instance != null &&
                Input.GetKeyDown(KeyBindManager.instance.slideKey))
            {
                CancelSlide();
            }
        }
        else
        {
            // EN MODE DÉPLACEMENT NORMAL 

            if (hasInput)
            {
                // On a un input : on accélère le momentum vers la vitesse cible
                horizontalMomentum = Vector3.Lerp(horizontalMomentum, move * moveSpeed, Time.deltaTime * momentumDecay);
            }
            else
            {
                // Plus d'input : le momentum décélère progressivement vers zéro
                horizontalMomentum = Vector3.Lerp(horizontalMomentum, Vector3.zero, Time.deltaTime * momentumDecay);
            }

            // On déplace le CharacterController avec le momentum calculé
            controller.Move(horizontalMomentum * Time.deltaTime);

            // Déclenchement de la glissade :
            // Conditions → touche slide pressée + input de direction + au sol
            if (KeyBindManager.instance != null &&
                Input.GetKeyDown(KeyBindManager.instance.slideKey) &&
                hasInput && isGrounded)
            {
                slideDirection    = horizontalMomentum.normalized; // On glisse dans la direction actuelle
                currentSlideSpeed = slideSpeed;
                if (slideCoroutine != null) StopCoroutine(slideCoroutine);
                slideCoroutine = StartCoroutine(SlideRoutine());
            }
        }

        // SAUT
        // Saut normal (au sol, pas en glissade)
        if (KeyBindManager.instance != null &&
            Input.GetKeyDown(KeyBindManager.instance.jumpKey) &&
            isGrounded && !isSliding)
        {
            // Formule physique classique : v = sqrt(h * 2 * |g|)
            velocity  = playerUp * Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravityValue));
            jumpTimer = 0.15f; // Délai avant de retester le sol
        }

        // Saut pendant une glissade → annule la glissade et saute
        if (KeyBindManager.instance != null &&
            Input.GetKeyDown(KeyBindManager.instance.jumpKey) &&
            isSliding)
        {
            CancelSlide();
            velocity  = playerUp * Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravityValue));
            jumpTimer = 0.15f;
        }

        // On applique la vélocité verticale (gravité + saut) au CharacterController
        controller.Move(velocity * Time.deltaTime);


        // CHANGEMENT DE GRAVITÉ
        // Si on appuie sur la touche de rotation de gravité et qu'aucune transition n'est en cours
        if (KeyBindManager.instance != null &&
            Input.GetKeyDown(KeyBindManager.instance.gravityRotateKey) &&
            !gravityManager.isTransitioning)
        {
            gravityManager.TriggerGravityFromCameraLook();
        }
    }

    // ANNULATION DE LA GLISSADE
    private void CancelSlide()
    {
        // On arrête la coroutine de glissade si elle tourne encore
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);

        // On restaure la hauteur du CharacterController (il avait été réduit de moitié)
        StartCoroutine(RestoreHeightRoutine());

        isSliding = false;

        // On conserve le momentum de glissade au moment de l'annulation
        // → le joueur garde sa vitesse et peut continuer à se déplacer
        horizontalMomentum = slideDirection * currentSlideSpeed;
    }

    // COROUTINE : DÉROULEMENT DE LA GLISSADE
    private IEnumerator SlideRoutine()
    {
        isSliding = true;

        // On réduit la hauteur du CharacterController pour simuler la posture basse
        float originalHeight = controller.height;
        controller.height = originalHeight / 2f;

        // On attend la durée maximale de la glissade
        yield return new WaitForSeconds(slideDuration);

        // Fin de la glissade : on restaure la hauteur
        controller.height = originalHeight;
        isSliding = false;
    }

    // COROUTINE : RESTAURATION DE LA HAUTEUR APRÈS ANNULATION
    private IEnumerator RestoreHeightRoutine()
    {
        // La hauteur avait été divisée par 2, donc on la recalcule en multipliant par 2
        float originalHeight = controller.height * 2f;

        // Petit délai pour éviter un conflit si SlideRoutine tourne encore
        yield return new WaitForSeconds(0.05f);

        controller.height = originalHeight;
    }
}
