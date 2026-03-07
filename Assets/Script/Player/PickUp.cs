using UnityEngine;

// PickUp.cs — Ramassage et lancer d'objets physiques
// Ce script permet au joueur de ramasser, tenir, déposer et
// lancer des objets avec un Rigidbody.
// Fonctionnement :
//   - Appuyer sur [F] en visant un objet → le ramasser
//   - Appuyer sur [F] en tenant un objet → le déposer
//   - Clic droit en tenant un objet      → le lancer
// L'objet est attiré vers un point de tenue (TempParent.cs)
// via un système de ressort physique (spring + damping).
// Quand l'objet est lâché, une gravité personnalisée lui est
// appliquée pour respecter la direction de gravité du jeu.
// À attacher sur : chaque GameObject ramassable (avec Rigidbody)

public class PickUp : MonoBehaviour
{
    bool isHolding = false;
    // true si le joueur tient actuellement cet objet

    // PARAMÈTRES
    [SerializeField] float throwForce = 15f;
    // Force appliquée lors du lancer

    [SerializeField] float maxDistance = 3f;
    // Distance maximale depuis laquelle le joueur peut ramasser l'objet

    [SerializeField] float springStrength = 15f;
    // Intensité de la force de rappel vers le point de tenue (TempParent)
    // Valeur haute = l'objet suit très vite et réactivement

    [SerializeField] float springDamping = 8f;
    // Amortissement du ressort : réduit les oscillations
    // Valeur haute = mouvement plus stable, moins de rebond

    // REFERENCES
    TempParent tempParent;          // Le point de tenue devant la caméra (TempParent.cs)
    Rigidbody rb;                   // Rigidbody de cet objet (pour les forces physiques)
    GravityManager gravityManager;  // Pour connaître la direction de gravité au moment du lâcher

    float capturedDistance;
    // Distance de l'objet au moment du ramassage → utilisée pour ajuster TempParent

    Coroutine gravityCoroutine;
    // Référence à la coroutine de gravité personnalisée (pour l'arrêter si besoin)

    void Start()
    {
        rb             = GetComponent<Rigidbody>();
        tempParent     = TempParent.Instance;
        gravityManager = FindFirstObjectByType<GravityManager>();
    }

    // BOUCLE PRINCIPALE - INPUTS
    void Update()
    {
        // Touche F : ramasser ou déposer
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isHolding)
                Drop();    // On tient l'objet → on le lâche
            else
                TryPickUp(); // On ne tient rien → on essaie de ramasser
        }

        // Clic droit : lancer l'objet si on le tient
        if (isHolding && Input.GetMouseButtonDown(1))
            Throw();
    }

    // BOUCLE PHYSIQUE — FORCE DE RESSORT
    void FixedUpdate()
    {
        // On ne fait rien si l'objet n'est pas tenu
        if (!isHolding) return;

        Vector3 targetPos  = tempParent.transform.position; // Position cible (devant la caméra)
        Vector3 currentPos = this.transform.position;       // Position actuelle de l'objet

        // Calcul du déplacement entre la position actuelle et la cible
        Vector3 displacement = targetPos - currentPos;

        // Force de rappel (ressort) : proportionnelle à la distance
        Vector3 springForce = displacement * springStrength;

        // Force d'amortissement : s'oppose à la vélocité pour stabiliser
        Vector3 dampingForce = -rb.linearVelocity * springDamping;

        // On applique les deux forces combinées
        rb.AddForce(springForce + dampingForce, ForceMode.Force);

        // On contrecarre la gravité Unity par défaut pour que seule notre
        // logique de ressort et notre gravité custom s'appliquent
        rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
    }

    // RAMASSAGE BOUCLE
    private void TryPickUp()
    {
        // On lance un Raycast depuis le centre de l'écran dans la direction du regard
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            // On vérifie que l'objet touché est bien CE script et que TempParent existe
            if (hit.transform == this.transform && tempParent != null)
            {
                // On stoppe la coroutine de gravité custom si elle tourne encore
                // (cas où l'objet venait d'être lâché et tombait encore)
                if (gravityCoroutine != null)
                {
                    StopCoroutine(gravityCoroutine);
                    gravityCoroutine = null;
                }

                // On mesure la distance initiale pour configurer le point de tenue
                capturedDistance = Vector3.Distance(Camera.main.transform.position, this.transform.position);

                isHolding             = true;
                rb.useGravity         = false;   // On désactive la gravité Unity (on gère la nôtre)
                rb.isKinematic        = false;   // On laisse la physique active (pour les forces)
                rb.detectCollisions   = true;    // L'objet reste collidable
                rb.linearVelocity     = Vector3.zero; // Reset de la vitesse pour éviter les sauts
                rb.angularVelocity    = Vector3.zero; // Reset de la rotation

                // On informe TempParent de la distance à maintenir
                tempParent.SetHoldDistance(capturedDistance);
            }
        }
    }

    // LANCER BOUCLE
    private void Throw()
    {
        isHolding = false;

        // Reset de la vélocité pour que le lancer soit propre et prévisible
        rb.linearVelocity = Vector3.zero;

        // On reprend la gravité custom (l'objet doit continuer à "tomber" dans la bonne direction)
        gravityCoroutine = StartCoroutine(ApplyCustomGravity());

        // On applique une force d'impulsion dans la direction du regard de la caméra
        Vector3 throwDirection = Camera.main.transform.forward;
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }

    // DEPOT BOUCLE
    private void Drop()
    {
        if (isHolding)
        {
            isHolding         = false;
            rb.linearVelocity = Vector3.zero; // L'objet ne rebondit pas au lâcher

            // On relance la gravité custom pour que l'objet tombe dans la bonne direction
            gravityCoroutine = StartCoroutine(ApplyCustomGravity());
        }
    }

    // COROUTINE : GRAVITÉ PERSONNALISÉE APRÈS LÂCHER
    // Au moment où l'objet est lâché/lancé, on capture la direction de gravité actuelle.
    // Cette direction est figée pour cet objet → elle ne changera plus même si le joueur
    // change la gravité plus tard.
    private System.Collections.IEnumerator ApplyCustomGravity()
    {
        // Capture de la gravité AU MOMENT du lâcher
        Vector3 gravityDir = gravityManager != null ?
            gravityManager.gravityDirection.normalized : Vector3.down;

        // Tant que l'objet n'est pas ramassé à nouveau, on applique la gravité
        while (!isHolding)
        {
            rb.AddForce(gravityDir * 9.81f, ForceMode.Acceleration);
            yield return null; // On attend la prochaine frame (FixedUpdate)
        }

        // Quand l'objet est ramassé, on réinitialise la référence
        gravityCoroutine = null;
    }
}
