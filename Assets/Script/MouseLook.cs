using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;      // Le corps du joueur (Parent)
    public Transform cameraTransform;  // La caméra (Enfant)
    public float mouseSensitivity = 2f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 1. Rotation Horizontale : On tourne autour de l'axe Y LOCAL du joueur.
        // C'est ça qui empêche de se retrouver la tête à l'envers.
        playerBody.Rotate(Vector3.up * mouseX);

        // 2. Rotation Verticale : On incline la caméra localement
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}