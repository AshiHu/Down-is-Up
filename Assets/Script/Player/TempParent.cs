using UnityEngine;

public class TempParent : MonoBehaviour
{
    public static TempParent Instance { get; private set; }
    [SerializeField] float holdDistance = 2f;
    private float currentHoldDistance;
    Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        currentHoldDistance = holdDistance;
    }

    public void SetHoldDistance(float distance)
    {
        currentHoldDistance = distance;
    }

    private void Update()
    {
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * currentHoldDistance;
        transform.rotation = mainCamera.transform.rotation;
    }
}