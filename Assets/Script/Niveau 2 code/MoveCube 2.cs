using UnityEngine;

public class MoveCube2 : MonoBehaviour
{
    [Header("Configuration Rotation")]
    [SerializeField] private float rotationSpeed = 50f;
    private Vector3 randomDirection;

    [Header("Configuration Mouvement")]
    [SerializeField] private float minMovementSpeed = 1f;
    [SerializeField] private float maxMovementSpeed = 5f;
    [SerializeField] private float maxDistance = 5f;

    private float movementSpeed; 

    [Header("Axe de déplacement")]
    [SerializeField] private bool moveOnX = false;
    [SerializeField] private bool moveOnY = true;
    [SerializeField] private bool moveOnZ = false;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;
        movementSpeed = Random.Range(minMovementSpeed, maxMovementSpeed);

        float randomOffset = Random.Range(-maxDistance, maxDistance);

        Vector3 randomDirectionOffset = Vector3.zero;
        if (moveOnX) randomDirectionOffset.x = randomOffset;
        if (moveOnY) randomDirectionOffset.y = randomOffset;
        if (moveOnZ) randomDirectionOffset.z = randomOffset;

        targetPosition = startPosition + randomDirectionOffset;

        float randomRotationX = Random.Range(-1f, 1f);
        float randomRotationY = Random.Range(-1f, 1f);
        float randomRotationZ = Random.Range(-1f, 1f);
        randomDirection = new Vector3(randomRotationX, randomRotationY, randomRotationZ).normalized;
    }

    void Update()
    {
        transform.Rotate(randomDirection * rotationSpeed * Time.deltaTime);
        float t = Mathf.PingPong(Time.time * movementSpeed, 1f);
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);
    }
}