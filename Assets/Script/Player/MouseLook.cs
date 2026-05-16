using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;         
    public Transform cameraTransform;   
    public float mouseSensitivity = 2f;  
    public GravityManager gravityManager;

    [Header("Smooth")]
    public float smoothSpeed = 15f;
    private float xRotation = 0f;
    private float smoothX = 0f; 
    private float smoothY = 0f; 


    void Start()
    {
        // Verrouille le curseur au centre de l'ecran et le rend invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        if (SettingsManager.IsOpen) return;

        // tourne la souris automatiquement quand on change de gravite
        float inputMultiplier = (gravityManager != null && gravityManager.isTransitioning) ? 0f : 1f;

        // Mouvement de la souris
        float targetMouseX = Input.GetAxis("Mouse X") * mouseSensitivity * inputMultiplier;
        float targetMouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * inputMultiplier;

        // smooth le mouvement de la souris
        smoothX = Mathf.Lerp(smoothX, targetMouseX, Time.deltaTime * smoothSpeed);
        smoothY = Mathf.Lerp(smoothY, targetMouseY, Time.deltaTime * smoothSpeed);

        // Applique la rotation au player et à la camera gauche/droite pour le player et haut/bas pour la camera
        playerBody.Rotate(Vector3.up * smoothX);
        xRotation -= smoothY;

        // bloque la camera du haut vers le bad pour éviter de faire un tour complet 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Applique la rotation à la camera de haut en bas
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
