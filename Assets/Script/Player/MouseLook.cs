using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public GravityManager gravityManager;

    [Header("Paramètres de Smooth")]
    public float smoothSpeed = 15f;

    private float xRotation = 0f;
    private float smoothX = 0f;
    private float smoothY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (gravityManager == null)
            gravityManager = FindFirstObjectByType<GravityManager>();
    }

    void Update()
    {
        if (SettingsManager.IsOpen) return;

        // On réduit le contrôle souris pendant la transition de gravité
        // pour éviter les conflits avec la rotation du GravityManager
        float inputMultiplier = (gravityManager != null && gravityManager.isTransitioning) ? 0f : 1f;

        float targetMouseX = Input.GetAxis("Mouse X") * mouseSensitivity * inputMultiplier;
        float targetMouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * inputMultiplier;

        smoothX = Mathf.Lerp(smoothX, targetMouseX, Time.deltaTime * smoothSpeed);
        smoothY = Mathf.Lerp(smoothY, targetMouseY, Time.deltaTime * smoothSpeed);

        // Rotation horizontale sur le up local du joueur
        playerBody.Rotate(Vector3.up * smoothX);

        // Rotation verticale caméra
        xRotation -= smoothY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}