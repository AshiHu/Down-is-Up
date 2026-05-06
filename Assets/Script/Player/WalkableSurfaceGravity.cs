using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WalkableSurfaceGravity : MonoBehaviour
{
    [Header("Références")]
    public GravityManager gravityManager;

    [Header("Paramètres de gravité")]
    // Force de gravité appliquée au joueur (synchronisée avec S_Perso.gravityValue au démarrage)
    [SerializeField] private float gravityStrength = 20f;
    // Vitesse de réalignement du joueur vers la nouvelle gravité (degrés/frame, style Slerp)
    [SerializeField] private float rotationSpeed   = 5f;

    [Header("Layer")]
    [SerializeField] private LayerMask walkableSurfaceLayer; // Assigner le layer "WalkableSurface" ici

    void Start()
    {
        // Synchronise gravityStrength avec S_Perso.gravityValue pour que le saut reste cohérent.
        // S_Perso attend une valeur négative (ex : -20) ; gravityStrength est positive (ex : 20).
        S_Perso movement = GetComponent<S_Perso>();
        if (movement != null) movement.gravityValue = -gravityStrength;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!IsWalkableSurface(hit.gameObject)) return;

        Vector3 normal = hit.normal;

        // Ignorer les normales trop faibles
        if (normal.sqrMagnitude < 0.001f) return;

        // Direction de gravité = opposé de la normale complète (X, Y et Z)
        Vector3 newGravityDir = -normal.normalized;

        gravityManager.UpdateGravityDirection(newGravityDir);
    }

    void Update()
    {
        if (gravityManager.isTransitioning) return;

        Vector3 targetUp = -gravityManager.gravityDirection;
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    private bool IsWalkableSurface(GameObject obj)
        => ((1 << obj.layer) & walkableSurfaceLayer) != 0;
}
