using UnityEngine;
using System.Collections;

public class GravityManager : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public PlayerMovementCustomKeys movement;
    public Camera playerCamera;

    [Header("Paramètres Transition")]
    public float transitionDuration = 0.7f;
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [HideInInspector]
    public Vector3 gravityDirection = Vector3.down;
    public bool isTransitioning = false;
    private Coroutine rotationCoroutine;

    private readonly Vector3[] cardinalDirections = new Vector3[]
    {
        Vector3.down, Vector3.up, Vector3.left,
        Vector3.right, Vector3.forward, Vector3.back
    };

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    public void TriggerGravityFromCameraLook()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 bestDirection = Vector3.down;
        float bestDot = -Mathf.Infinity;

        foreach (Vector3 dir in cardinalDirections)
        {
            float dot = Vector3.Dot(cameraForward, dir);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestDirection = dir;
            }
        }

        if (bestDirection == gravityDirection) return;
        StartGravityTransition(bestDirection);
    }

    void StartGravityTransition(Vector3 newGravity)
    {
        gravityDirection = newGravity.normalized;
        if (movement != null) movement.velocity = Vector3.zero;
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(SmoothRotationRoutine(newGravity));
    }

    IEnumerator SmoothRotationRoutine(Vector3 newGravity)
    {
        isTransitioning = true;

        Vector3 targetUp = -newGravity.normalized;
        Quaternion startRotation = player.rotation;

        Vector3 forward = Vector3.ProjectOnPlane(player.forward, targetUp);
        if (forward.sqrMagnitude < 0.01f)
            forward = Vector3.ProjectOnPlane(player.right, targetUp);

        Quaternion targetRotation = Quaternion.LookRotation(forward, targetUp);
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = rotationCurve.Evaluate(Mathf.Clamp01(elapsed / transitionDuration));
            player.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        player.rotation = targetRotation;
        isTransitioning = false;
    }
}