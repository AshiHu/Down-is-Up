using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    [Header("References")]
    public S_Perso playerScript;
    private Camera cam;

    [Header("Parametres FOV")]
    public float baseFOV = 60f;
    public float maxFOV = 90f;

    [Header("Vitesse de transition")]
    public float smoothSpeed = 2f;

    void Start()
    {
        // Récupérer la caméra attachée à ce GameObject
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (playerScript == null) return;
        float targetFOV = baseFOV;

        //on regarde si le joueur est en mouvement
        bool isMoving = Input.GetAxisRaw(playerScript.horizontalAxis) != 0 ||
                        Input.GetAxisRaw(playerScript.verticalAxis)   != 0;

        // Si le joueur est en mouvement, on augmente le FOV
        if (isMoving) targetFOV = baseFOV + 10f;
        if (KeyBindManager.instance != null && Input.GetKey(KeyBindManager.instance.slideKey))
            targetFOV = maxFOV;

        // Transition fluide du FOV vers la valeur cible
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
    }
}
