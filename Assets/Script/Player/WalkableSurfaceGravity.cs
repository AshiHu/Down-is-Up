using UnityEngine;

// WalkableSurfaceGravity.cs — Gravité basée sur les surfaces de contact
//
// Comment assigner le Layer "WalkableSurface" dans l'éditeur Unity :
//   1. Edit > Project Settings > Tags and Layers
//   2. Dans la section "Layers", choisir un slot vide et le nommer "WalkableSurface"
//   3. Sélectionner chaque surface/mur marchable dans la Hierarchy
//   4. En haut de l'Inspector, changer le champ "Layer" en "WalkableSurface"
//   5. Sur ce composant (WalkableSurfaceGravity), assigner ce même layer dans le champ "Walkable Surface Layer"
//
// Pré-requis sur le GameObject joueur :
//   - Ce script doit être sur le même GameObject que CharacterController et S_Perso
//   - Un GravityManager assigné dans l'Inspector (champ gravityManager)
//   - Dans S_Perso, la section "RÉALIGNEMENT DU CORPS" est désactivée :
//     ce script en est maintenant le seul responsable (via rotationSpeed)

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

    // OnControllerColliderHit est le callback de collision du CharacterController.
    // Il se déclenche chaque frame où CharacterController.Move() entre en contact avec un collider.
    // C'est l'équivalent de OnCollisionStay pour un Rigidbody.
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Ignorer les surfaces qui ne sont pas sur le layer WalkableSurface
        if (!IsWalkableSurface(hit.gameObject)) return;

        // Normale sortante de la surface au point de contact
        Vector3 normal = hit.normal;

        // On extrait uniquement X et Y — l'axe Z ne sert jamais de direction de gravité
        Vector3 xyNormal = new Vector3(normal.x, normal.y, 0f);

        // Ignorer les normales qui s'annulent après la suppression du Z (surface face-Z pure)
        if (xyNormal.sqrMagnitude < 0.001f) return;

        // Direction de gravité = opposé de la normale XY normalisée (attirer vers la surface)
        // Exemple : normal (0,1,0) → gravité (0,-1,0) | normal (1,0,0) → gravité (-1,0,0)
        Vector3 newGravityDir = -xyNormal.normalized;

        // Met à jour la direction dans GravityManager — S_Perso, PickUp, etc. se synchronisent automatiquement
        // Si le joueur n'est plus en contact (plus d'appel ici), la dernière direction est conservée
        gravityManager.UpdateGravityDirection(newGravityDir);
    }

    void Update()
    {
        // Rotation fluide du joueur : aligne son axe "haut" sur l'opposé de la gravité.
        // On cède le contrôle à GravityManager pendant ses propres transitions (touche E)
        // pour éviter que les deux systèmes se battent sur transform.rotation.
        if (gravityManager.isTransitioning) return;

        Vector3 targetUp = -gravityManager.gravityDirection;
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    // Vérifie si un GameObject appartient au layer WalkableSurface via bitmask
    private bool IsWalkableSurface(GameObject obj)
        => ((1 << obj.layer) & walkableSurfaceLayer) != 0;
}
