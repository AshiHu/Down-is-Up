using UnityEngine;

public class TempParent : MonoBehaviour
{
    public static TempParent Instance { get; private set; }

    [SerializeField] float holdDistance = 2f; // Distance devant la caméra
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
    }

    private void Update()
    {
        // Le TempParent suit toujours la caméra à holdDistance devant elle
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * holdDistance;
        transform.rotation = mainCamera.transform.rotation;
    }
}