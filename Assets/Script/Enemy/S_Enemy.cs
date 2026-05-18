using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class S_Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;                // Vitesse de déplacement de l'ennemi
    public Transform[] patrolPoints;           // Liste des points de patrouille (Transform)
    public float pauseDuration = 2f;           // Durée de la pause à chaque point de patrouille

    // Variables pour la détection du joueur
    public float radius = 10f;                 // Rayon de détection
    public float angle = 60f;                  // Angle de détection
    public LayerMask targetMask;               // Couches des cibles à détecter (ici le joueur)
    public LayerMask obstructionMask;          // Couches des obstructions (murs, obstacles)
    public GameObject playerRef;               // Référence au joueur
    public bool canSeePlayer;                  // Indicateur si l'ennemi voit le joueur

    private NavMeshAgent agent;                // NavMeshAgent de l'ennemi
    private Transform targetPoint;             // Point cible de patrouille actuel
    private bool isPausing = false;            // Indicateur de pause en cours
    private bool isChasingPlayer = false;      // Indicateur si l'ennemi poursuit le joueur

    private void Start()
    {
        // Initialisation du NavMeshAgent pour gérer les déplacements
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        // Trouver le joueur par son tag
        playerRef = GameObject.FindGameObjectWithTag("Player");

        // Vérifier si des points de patrouille existent
        if (patrolPoints.Length > 0)
        {
            ChooseRandomPatrolPoint();  // Sélectionner un point de patrouille aléatoire
        }

        // Démarrer la routine de vérification du champ de vision
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        // Attendre 0.2 secondes entre chaque vérification
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        // Boucle infinie pour vérifier en continu
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        // Vérifier les objets dans le rayon de détection avec la couche "targetMask" (ici le joueur)
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        // Si des cibles sont trouvées dans la zone
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;  // Cible trouvée
            Vector3 directionToTarget = (target.position - transform.position).normalized;  // Direction vers la cible

            // Vérifier si la cible est dans le champ de vision (angle)
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);  // Distance vers la cible

                // Vérifier s'il n'y a pas d'obstacles entre l'ennemi et le joueur
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;  // L'ennemi voit le joueur
                }
                else
                {
                    canSeePlayer = false;  // Un obstacle bloque la vue
                }
            }
            else
            {
                canSeePlayer = false;  // La cible est en dehors du champ de vision
            }
        }
        else
        {
            canSeePlayer = false;  // Aucune cible dans la zone de détection
        }
    }

    private void Update()
    {
        // Si l'ennemi peut voir le joueur
        if (canSeePlayer && !isPausing)
        {
            // L'ennemi poursuit le joueur
            isChasingPlayer = true;
            agent.SetDestination(playerRef.transform.position);  // Met à jour la destination du NavMeshAgent
        }
        else if (!canSeePlayer && isChasingPlayer)
        {
            // Si l'ennemi ne voit plus le joueur, il arrête la poursuite et retourne à la patrouille
            isChasingPlayer = false;
            StartCoroutine(PauseAndPatrol());  // Reprendre la patrouille
        }

        // Si l'ennemi a atteint sa destination (point de patrouille) et n'est pas en train de poursuivre le joueur
        if (!isPausing && !isChasingPlayer && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(PauseAndPatrol());  // Démarrer la pause, puis passer au prochain point
        }
    }

    private IEnumerator PauseAndPatrol()
    {
        // Indiquer que l'ennemi est en pause
        isPausing = true;

        // Attendre pendant la durée de la pause
        yield return new WaitForSeconds(pauseDuration);

        // Sélectionner un point de patrouille aléatoire
        ChooseRandomPatrolPoint();

        // Reprendre le mouvement vers le nouveau point
        agent.SetDestination(targetPoint.position);

        // Fin de la pause
        isPausing = false;
    }

    private void ChooseRandomPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, patrolPoints.Length);  // Obtenir un index aléatoire
            targetPoint = patrolPoints[randomIndex];  // Sélectionner le point correspondant
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dessiner les points de patrouille dans l'éditeur pour les visualiser
        if (patrolPoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            foreach (Transform patrolPoint in patrolPoints)
            {
                Gizmos.DrawSphere(patrolPoint.position, 0.3f);  // Dessiner une sphère pour chaque point de patrouille
            }
        }

        // Dessiner le rayon de détection dans l'éditeur
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);  // Sphère de détection

        // Dessiner le champ de vision dans l'éditeur
        Gizmos.color = Color.green;
        Vector3 leftBoundary = Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius;
        Vector3 rightBoundary = Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
