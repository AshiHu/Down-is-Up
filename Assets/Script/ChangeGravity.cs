using UnityEngine;

public class GravityWheelSimple : MonoBehaviour
{
    [Header("UI flèches")]
    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject arrowLeft;
    public GameObject arrowRight;

    [Header("Clé pour ouvrir la roue")]
    public KeyCode openWheelKey = KeyCode.E;

    [Header("Temps")]
    public float slowTimeScale = 0.2f;

    [Header("Références")]
    public MouseLook mouseLookScript;   // Script MouseLook
    public GravityManager gravityManager;   // Script annexe gravité

    private bool wheelOpen = false;
    private float originalTimeScale;

    void Update()
    {
        // Ouvrir / fermer la roue
        if (Input.GetKeyDown(openWheelKey))
        {
            if (!wheelOpen)
                OpenWheel();
            else
                CloseWheel();
        }

        if (!wheelOpen) return;

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

    void OpenWheel()
    {
        wheelOpen = true;

        // UI
        SetArrowVisibility(true);

        // Temps ralenti
        originalTimeScale = Time.timeScale;
        Time.timeScale = slowTimeScale;

        // Bloquer la vue
        if (mouseLookScript != null)
            mouseLookScript.enabled = false;

        // Curseur libre
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseWheel()
    {
        wheelOpen = false;

        // UI
        SetArrowVisibility(false);

        // Temps normal
        Time.timeScale = originalTimeScale;

        // Réactiver la vue
        if (mouseLookScript != null)
            mouseLookScript.enabled = true;

        // Curseur verrouillé
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private bool IsMouseOver(GameObject uiElement, Vector2 mousePos)
    {
        if (uiElement == null) return false;

        RectTransform rt = uiElement.GetComponent<RectTransform>();
        Vector2 pos = rt.position;
        Vector2 size = rt.sizeDelta;

        Rect rect = new Rect(
            pos.x - size.x / 2f,
            pos.y - size.y / 2f,
            size.x,
            size.y

        );

        return rect.Contains(mousePos);
    }

    private void SetArrowVisibility(bool visible)
    {
        arrowUp.SetActive(visible);
        arrowDown.SetActive(visible);
        arrowLeft.SetActive(visible);
        arrowRight.SetActive(visible);
    }
}
