using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WalkableSurfaceGravity : MonoBehaviour
{
    [Header("Références")]
    public GravityManager gravityManager;

    [Header("Paramètres de gravité")]
    [SerializeField] private float gravityStrength = 20f;
    [SerializeField] private float rotationSpeed   = 5f;

    [Header("Layer")]
    [SerializeField] private LayerMask walkableSurfaceLayer; 

    void Start()
    {
        S_Perso movement = GetComponent<S_Perso>();
        if (movement != null) movement.gravityValue = -gravityStrength;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Verifie si la surface touchee est dans le layer des surfaces marchables
        if (!IsWalkableSurface(hit.gameObject)) return;

        // quand on est sur une surface ou on peut marcher on applique la gravité dasn la direction opposée à la normale de la surface
        Vector3 normal = hit.normal;
        if (normal.sqrMagnitude < 0.001f) return;
        Vector3 newGravityDir = -normal.normalized;

        float angle = Vector3.Angle(gravityManager.gravityDirection, newGravityDir);
        if (angle > 5f && S_SoundManager.instance != null)
            S_SoundManager.instance.PlayGravityChange();

        gravityManager.UpdateGravityDirection(newGravityDir);
    }

    void Update()
    {
        // on ne touche pas a la rotation de personnage on le fait dans le script MouseLook 
        if (gravityManager.isTransitioning) return;

        // gestion de la rotation du personnage pour qu'il s'aligne avec la direction de gravite

        // targetup mettre la tete du perso a l'oppose de la surface marchable
        Vector3 targetUp = -gravityManager.gravityDirection;

        // calcul de la rotation cible pour aligner le haut du personnage avec la direction de gravite
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;

        // fait une interpolation sphérique pour que la rotation soit progressive et fluid
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    private bool IsWalkableSurface(GameObject obj)
        => ((1 << obj.layer) & walkableSurfaceLayer) != 0;
}
