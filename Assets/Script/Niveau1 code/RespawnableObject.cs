using UnityEngine;

public class RespawnableObject : MonoBehaviour
{
    public Transform respawnPoint;
    public float respawnDelay = 10f;

    private Rigidbody _rb;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _initialPosition = respawnPoint != null ? respawnPoint.position : transform.position;
        _initialRotation = respawnPoint != null ? respawnPoint.rotation : transform.rotation;
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(Respawn), respawnDelay, respawnDelay);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Respawn));
    }

    public void Respawn()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.position = respawnPoint != null ? respawnPoint.position : _initialPosition;
        transform.rotation = respawnPoint != null ? respawnPoint.rotation : _initialRotation;
    }
}