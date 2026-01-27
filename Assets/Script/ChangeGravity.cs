using UnityEngine;

public class GravityWheelSimple : MonoBehaviour
{
    [Header("UI flèches")]
    public RectTransform wheelPivot; // L'objet parent qui contient les 4 flèches
    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject arrowLeft;
    public GameObject arrowRight;

    [Header("Clé pour ouvrir la roue")]
    public KeyCode openWheelKey = KeyCode.E;

    [Header("Temps")]
    public float slowTimeScale = 0.2f;

    [Header("Références")]
    public MouseLook mouseLookScript;
    public GravityManager gravityManager;

    private bool wheelOpen = false;
    private float originalTimeScale = 1f;

    void Update()
    {
        // Ouvrir / fermer la roue
        if (Input.GetKeyDown(openWheelKey))
        {
            if (!wheelOpen) OpenWheel();
            else CloseWheel();
        }

        if (!wheelOpen) return;

        // Mise à jour de la rotation pour compenser celle de la caméra
        UpdateWheelRotation();

        // Sélection avec clic gauche
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            if (IsMouseOver(arrowUp, mousePos))
            {
                gravityManager.SetGravityUp();
                CloseWheel();
            }
            else if (IsMouseOver(arrowDown, mousePos))
            {
                gravityManager.SetGravityDown();
                CloseWheel();
            }
            else if (IsMouseOver(arrowLeft, mousePos))
            {
                gravityManager.SetGravityLeft();
                CloseWheel();
            }
            else if (IsMouseOver(arrowRight, mousePos))
            {
                gravityManager.SetGravityRight();
                CloseWheel();
            }
        }
    }

    void UpdateWheelRotation()
    {
        if (wheelPivot != null)
        {
            // On récupère l'angle Z de la caméra
            float currentZ = Camera.main.transform.eulerAngles.z;
            // On applique l'inverse au pivot pour que les flèches restent "droites" par rapport au monde
            wheelPivot.rotation = Quaternion.Euler(0, 0, -currentZ);
        }
    }

    void OpenWheel()
    {
        wheelOpen = true;
        SetArrowVisibility(true);
        UpdateWheelRotation(); // Aligner immédiatement à l'ouverture

        originalTimeScale = Time.timeScale;
        Time.timeScale = slowTimeScale;

        if (mouseLookScript != null) mouseLookScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseWheel()
    {
        wheelOpen = false;
        SetArrowVisibility(false);

        Time.timeScale = originalTimeScale;

        if (mouseLookScript != null) mouseLookScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private bool IsMouseOver(GameObject uiElement, Vector2 mousePos)
    {
        if (uiElement == null) return false;

        RectTransform rt = uiElement.GetComponent<RectTransform>();

        // Utilisation de RectangleContainsScreenPoint pour gérer la rotation du RectTransform
        return RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null);
    }

    private void SetArrowVisibility(bool visible)
    {
        if (arrowUp) arrowUp.SetActive(visible);
        if (arrowDown) arrowDown.SetActive(visible);
        if (arrowLeft) arrowLeft.SetActive(visible);
        if (arrowRight) arrowRight.SetActive(visible);
    }
}