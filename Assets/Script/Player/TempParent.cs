using UnityEngine;

public class TempParent : MonoBehaviour
{
    public static TempParent Instance { get; private set; }
    [SerializeField] float holdDistance = 2f;
    private float currentHoldDistance;
    Camera mainCamera;

    // creation du singleton pour avoir un seul script temps parent dans la scene
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this); 
    }

    private void Start()
    {
        // on donne la distance du hold au debut pour que ça soit pas 0
        mainCamera = Camera.main;
        currentHoldDistance = holdDistance; 
    }

    public void SetHoldDistance(float distance)
    {
        currentHoldDistance = distance;
    }

    private void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (mainCamera == null) return;

        transform.position = mainCamera.transform.position
                           + mainCamera.transform.forward * currentHoldDistance;

        transform.rotation = mainCamera.transform.rotation;
    }
}
