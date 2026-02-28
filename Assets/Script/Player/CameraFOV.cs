using UnityEngine;

public class SimpleSpeedFOV : MonoBehaviour
{
    [Header("Références")]
    public PlayerMovementCustomKeys playerScript;
    private Camera cam;

    [Header("Paramètres FOV")]
    public float baseFOV = 60f;
    public float maxFOV = 90f;

    [Header("Vitesse de transition")]
    [Tooltip("Plus la valeur est petite, plus c'est lent")]
    public float smoothSpeed = 2f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (playerScript == null)
            playerScript = FindFirstObjectByType<PlayerMovementCustomKeys>();
    }

    void Update()
    {
        if (playerScript == null) return;

        float targetFOV = baseFOV;

        bool isMoving = Input.GetAxisRaw(playerScript.horizontalAxis) != 0 ||
                        Input.GetAxisRaw(playerScript.verticalAxis) != 0;

        if (isMoving) targetFOV = baseFOV + 10f;

        if (KeyBindManager.instance != null && Input.GetKey(KeyBindManager.instance.slideKey))
            targetFOV = maxFOV;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
    }
}