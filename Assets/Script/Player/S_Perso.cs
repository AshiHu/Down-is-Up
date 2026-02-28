using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementCustomKeys : MonoBehaviour
{
    [Header("Physique")]
    public float moveSpeed = 8f;
    public float jumpHeight = 2.2f;
    public float gravityValue = -20f;

    [Header("Momentum")]
    public float momentumDecay = 6f; // Décélération (plus c'est haut, plus c'est rapide)

    [Header("Glissade")]
    public float slideSpeed = 15f;
    public float slideDuration = 0.8f;
    public float slideDecay = 4f; // Décélération fin de glissade
    public bool isSliding = false;
    private Vector3 slideDirection;
    private float currentSlideSpeed;
    private Coroutine slideCoroutine;

    [Header("Configuration des Axes")]
    public string verticalAxis = "Vertical";
    public string horizontalAxis = "Horizontal";

    [Header("Détection Sol")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Références")]
    public GravityManager gravityManager;

    private CharacterController controller;
    [HideInInspector] public Vector3 velocity;
    private Vector3 horizontalMomentum; // Momentum horizontal conservé
    private bool isGrounded;
    private float jumpTimer = 0f;

    void Start() => controller = GetComponent<CharacterController>();

    void Update()
    {
        if (gravityManager == null) return;

        Vector3 gravityDir = gravityManager.gravityDirection.normalized;
        Vector3 playerUp = -gravityDir;

        // --- RÉALIGNEMENT DU CORPS ---
        if (transform.up != playerUp)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, playerUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // --- DÉTECTION DU SOL ---
        if (jumpTimer <= 0)
        {
            float rayLength = (controller.height / 2f) + groundCheckDistance;
            isGrounded = Physics.Raycast(transform.position, gravityDir, rayLength, groundLayer);
            Debug.DrawRay(transform.position, gravityDir * rayLength, isGrounded ? Color.green : Color.red);
        }
        else
        {
            isGrounded = false;
            jumpTimer -= Time.deltaTime;
        }

        // --- GESTION VÉLOCITÉ VERTICALE ---
        if (isGrounded)
        {
            float dot = Vector3.Dot(velocity, gravityDir);
            if (dot > 0) velocity = gravityDir * 2f;
        }
        else
        {
            velocity += gravityDir * -gravityValue * Time.deltaTime;
        }

        // --- INPUTS ---
        float inputX = Input.GetAxisRaw(horizontalAxis);
        float inputZ = Input.GetAxisRaw(verticalAxis);

        Vector3 moveForward = Vector3.ProjectOnPlane(transform.forward, gravityDir).normalized;
        Vector3 moveRight = Vector3.ProjectOnPlane(transform.right, gravityDir).normalized;
        Vector3 move = (moveRight * inputX + moveForward * inputZ).normalized;
        bool hasInput = move.sqrMagnitude > 0.1f;

        // --- LOGIQUE DE DÉPLACEMENT ---
        if (isSliding)
        {
            // Décélération progressive pendant la glissade
            currentSlideSpeed = Mathf.Lerp(currentSlideSpeed, 0f, Time.deltaTime * slideDecay);
            controller.Move(slideDirection * currentSlideSpeed * Time.deltaTime);

            // Mise à jour du momentum pour le conserver si on annule
            horizontalMomentum = slideDirection * currentSlideSpeed;

            // Annulation slide avec la touche slide ou en sautant
            if (KeyBindManager.instance != null &&
                Input.GetKeyDown(KeyBindManager.instance.slideKey))
            {
                CancelSlide();
            }
        }
        else
        {
            if (hasInput)
            {
                // On bouge : on applique le mouvement et on met à jour le momentum
                horizontalMomentum = Vector3.Lerp(horizontalMomentum, move * moveSpeed, Time.deltaTime * momentumDecay);
            }
            else
            {
                // On lâche la touche : le momentum décélère progressivement
                horizontalMomentum = Vector3.Lerp(horizontalMomentum, Vector3.zero, Time.deltaTime * momentumDecay);
            }

            controller.Move(horizontalMomentum * Time.deltaTime);

            // Déclenchement glissade
            if (KeyBindManager.instance != null &&
                Input.GetKeyDown(KeyBindManager.instance.slideKey) &&
                hasInput && isGrounded)
            {
                slideDirection = horizontalMomentum.normalized;
                currentSlideSpeed = slideSpeed;
                if (slideCoroutine != null) StopCoroutine(slideCoroutine);
                slideCoroutine = StartCoroutine(SlideRoutine());
            }
        }

        // --- SAUT ---
        if (KeyBindManager.instance != null &&
            Input.GetKeyDown(KeyBindManager.instance.jumpKey) &&
            isGrounded && !isSliding)
        {
            velocity = playerUp * Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravityValue));
            jumpTimer = 0.15f;
        }

        // Annulation slide par saut
        if (KeyBindManager.instance != null &&
            Input.GetKeyDown(KeyBindManager.instance.jumpKey) &&
            isSliding)
        {
            CancelSlide();
            velocity = playerUp * Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravityValue));
            jumpTimer = 0.15f;
        }

        controller.Move(velocity * Time.deltaTime);

        // --- ROTATION GRAVITÉ ---
        if (KeyBindManager.instance != null &&
            Input.GetKeyDown(KeyBindManager.instance.gravityRotateKey) &&
            !gravityManager.isTransitioning)
        {
            gravityManager.TriggerGravityFromCameraLook();
        }
    }

    private void CancelSlide()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        StartCoroutine(RestoreHeightRoutine());
        isSliding = false;
        // On garde le momentum de la glissade au moment de l'annulation
        horizontalMomentum = slideDirection * currentSlideSpeed;
    }

    private IEnumerator SlideRoutine()
    {
        isSliding = true;
        float originalHeight = controller.height;
        controller.height = originalHeight / 2f;
        yield return new WaitForSeconds(slideDuration);
        controller.height = originalHeight;
        isSliding = false;
    }

    private IEnumerator RestoreHeightRoutine()
    {
        float originalHeight = controller.height * 2f;
        yield return new WaitForSeconds(0.05f);
        controller.height = originalHeight;
    }
}