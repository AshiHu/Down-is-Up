using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class S_Perso : MonoBehaviour
{
    [Header("Physique")]
    public float moveSpeed = 8f;
    public float jumpHeight = 2.2f;
    public float gravityValue = -20f;

    [Header("Momentum")]
    public float momentumDecay = 6f;

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
    private Vector3 horizontalMomentum;
    private float originalHeight;
    private bool isGrounded;
    private float jumpTimer = 0f;

    void Start()
    {
        // recupere le character controller et la hauteur originale pour pouvoir la reset quand on change de gravite
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
    }

    void Update()
    {
        // Si le gravity manager n'est pas assigne, on ne fait rien pour eviter les erreurs
        if (gravityManager == null) return;

        // direction vers laquelle le personnage tombe 
        Vector3 gravityDir = gravityManager.gravityDirection.normalized;

        // haut du personnage, utilise pour determiner la direction du saut et pour faire les raycast de detection de sol
        Vector3 playerUp = -gravityDir;

        // on tire un raycast qui part du joueur vers la bas grace gravityDir
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

        // si le joueur est sur le sol on met la force vers le bas a 2 pour pas accumuler une vitesse de chute enorme
        if (isGrounded)
        {
            float dot = Vector3.Dot(velocity, gravityDir);
            if (dot > 0) velocity = gravityDir * 2f;
        }
        else
        {
            // quand on est dans les airs on applique la gravite normalement
            velocity += gravityDir * -gravityValue * Time.deltaTime;
        }

        // recupere les inputs du joueur 
        float inputX = Input.GetAxisRaw(horizontalAxis);
        float inputZ = Input.GetAxisRaw(verticalAxis);

        // on fait en fonction de la directipon du joueur et de la gravite pour que les controles soient toujours coherents peu importe la direction de la gravite
        Vector3 moveForward = Vector3.ProjectOnPlane(transform.forward, gravityDir).normalized;
        Vector3 moveRight = Vector3.ProjectOnPlane(transform.right, gravityDir).normalized;
        Vector3 move = (moveRight * inputX + moveForward * inputZ).normalized;
        bool hasInput = move.sqrMagnitude > 0.1f;

        // momentum calcule en fonction des input 
        if (hasInput)
            horizontalMomentum = Vector3.Lerp(horizontalMomentum, move * moveSpeed, Time.deltaTime * momentumDecay);
        else
            horizontalMomentum = Vector3.Lerp(horizontalMomentum, Vector3.zero, Time.deltaTime * momentumDecay);
        controller.Move(horizontalMomentum * Time.deltaTime);

        // recupere la touche pour sauter depuis le KeyBindManager
        if (KeyBindManager.instance != null &&
            Input.GetKeyDown(KeyBindManager.instance.jumpKey) &&
            isGrounded)
        {
            // calcule du saut:
            // - vitesse initiale = √(hauteur × 2 × gravite)
            velocity = playerUp * Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravityValue));
            jumpTimer = 0.15f;
            if (S_SoundManager.instance != null)
                S_SoundManager.instance.PlayJump();

        }

        controller.Move(velocity * Time.deltaTime);
    }

    // reset tout quand on meurt ou que l'on change de niveau 
    public void ResetMovement()
    {
        velocity = Vector3.zero;
        horizontalMomentum = Vector3.zero;
        controller.height = originalHeight;
    }
}