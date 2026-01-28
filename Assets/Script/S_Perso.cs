using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementCustomKeys : MonoBehaviour
{
    [Header("Physique")]
    public float moveSpeed = 8f;
    public float jumpHeight = 2.2f;
    public float gravityValue = -20f;

    [Header("Glissade")]
    public float slideSpeed = 15f;
    public float slideDuration = 0.8f;
    public bool isSliding = false;
    private Vector3 slideDirection;

    [Header("Configuration des Touches")]
    public string verticalAxis = "Vertical";
    public string horizontalAxis = "Horizontal";
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.LeftControl;

    [Header("Détection Sol")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Références")]
    public GravityManager gravityManager;

    private CharacterController controller;
    [HideInInspector] public Vector3 velocity;
    private bool isGrounded;
    private float jumpTimer = 0f;

    void Start() => controller = GetComponent<CharacterController>();

    void Update()
    {
        if (gravityManager == null) return;

        Vector3 gravityDir = gravityManager.gravityDirection.normalized;
        Vector3 playerUp = -gravityDir;

        // --- FIX : RÉALIGNEMENT DU CORPS ---
        // On aligne le "haut" du joueur avec l'opposé de la gravité.
        // Cela empêche de rentrer dans les murs lors des changements de surface.
        if (transform.up != playerUp)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, playerUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // --- DÉTECTION DU SOL (PIEDS UNIQUEMENT) ---
        if (jumpTimer <= 0)
        {
            float rayLength = (controller.height / 2f) + groundCheckDistance;
            // On part d'un peu plus haut que le centre pour être sûr
            isGrounded = Physics.Raycast(transform.position, gravityDir, rayLength, groundLayer);
            Debug.DrawRay(transform.position, gravityDir * rayLength, isGrounded ? Color.green : Color.red);
        }
        else
        {
            isGrounded = false;
            jumpTimer -= Time.deltaTime;
        }

        // --- GESTION VÉLOCITÉ ---
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

        // --- LOGIQUE DE DÉPLACEMENT ---
        if (isSliding)
        {
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(move * moveSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyBindManager.instance.slideKey) && move.sqrMagnitude > 0.1f && isGrounded)
            {
                slideDirection = move;
                StartCoroutine(SlideRoutine());
            }
        }

        // --- SAUT ---
        if (Input.GetKeyDown(KeyBindManager.instance.jumpKey) && isGrounded && !isSliding)
        {
            velocity = playerUp * Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravityValue));
            jumpTimer = 0.15f;
        }

        controller.Move(velocity * Time.deltaTime);
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
}