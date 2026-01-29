using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;

    [Header("Paramètres de Smooth")]
    public float smoothSpeed = 15f;

    private float xRotation = 0f; // Haut/Bas
    private float yRotation = 0f; // Gauche/Droite

    // Variables pour le lissage
    private float smoothX = 0f;
    private float smoothY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (SettingsManager.IsOpen) return;
        // 1. Récupération brute des entrées souris
        float targetMouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float targetMouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 2. Lissage des valeurs d'entrée (Le "Smooth")
        // On lisse les nombres, pas encore les rotations, pour ne pas entrer en conflit avec la gravité
        smoothX = Mathf.Lerp(smoothX, targetMouseX, Time.deltaTime * smoothSpeed);
        smoothY = Mathf.Lerp(smoothY, targetMouseY, Time.deltaTime * smoothSpeed);

        // 3. Rotation Horizontale (Gauche/Droite)
        // On applique la rotation sur l'axe UP local du joueur (géré par le GravityManager)
        playerBody.Rotate(Vector3.up * smoothX);

        // 4. Rotation Verticale (Haut/Bas)
        xRotation -= smoothY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // On applique uniquement la rotation X locale pour ne pas écraser les changements du GravityManager
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}