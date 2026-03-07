using UnityEngine;

// TempParent.cs — Point de maintien des objets ramassés
// Ce script gère la position et la rotation du "point de tenue"
// devant la caméra. C'est le point cible vers lequel l'objet
// ramassé (PickUp.cs) est attiré via une force de ressort.
// Fonctionnement :
//   - Ce GameObject suit en permanence la caméra, à une distance
//     définie (holdDistance), dans la direction du regard.
//   - PickUp.cs utilise la position de ce Transform comme cible.
//   - Il s'agit d'un Singleton (une seule instance dans la scène).
// À attacher sur : un GameObject vide enfant de la scène
//                  (pas enfant de la caméra, pour éviter les conflits physiques)

public class TempParent : MonoBehaviour
{
    // SINGLETON
    public static TempParent Instance { get; private set; }
    // Permet à PickUp.cs d'accéder à ce script sans référence directe :
    // TempParent.Instance.transform.position

    // PARAMÈTRES
    [SerializeField] float holdDistance = 2f;
    // Distance par défaut devant la caméra où l'objet est tenu
    // Peut être ajustée dynamiquement via SetHoldDistance()

    private float currentHoldDistance;
    // Distance effective actuelle (peut varier selon l'objet ramassé)

    // RÉFÉRENCES
    Camera mainCamera;
    // Référence à la caméra principale pour suivre sa position/rotation

    // INITIALISATION DU SINGLETON
    private void Awake()
    {
        // Implémentation du pattern Singleton :
        // Si aucune instance n'existe, on s'enregistre comme instance unique
        if (Instance == null)
            Instance = this;
        else
            Destroy(this); // Si une instance existe déjà, on détruit ce doublon
    }

    private void Start()
    {
        mainCamera = Camera.main;
        currentHoldDistance = holdDistance; // On part de la valeur par défaut
    }

    // MODIFICATION DE LA DISTANCE
    // Appelé par PickUp.cs lors du ramassage d'un objet.
    // On adapte la distance de tenue à la distance initiale de l'objet au ramassage,
    // pour qu'il ne "saute" pas à un endroit fixe.
    public void SetHoldDistance(float distance)
    {
        currentHoldDistance = distance;
    }

    private void Update()
    {
        // Ce GameObject se place toujours devant la caméra, à currentHoldDistance
        // → c'est le "point de visée" que les objets ramassés cherchent à atteindre
        transform.position = mainCamera.transform.position
                           + mainCamera.transform.forward * currentHoldDistance;

        // On copie aussi la rotation de la caméra pour que l'objet s'oriente correctement
        transform.rotation = mainCamera.transform.rotation;
    }
}
