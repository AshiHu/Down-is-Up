using UnityEngine;

public class PickUp : MonoBehaviour
{
    bool isHolding = false;

    [SerializeField] float throwForce = 15f;
    [SerializeField] float maxDistance = 3f;
    [SerializeField] float springStrength = 15f;
    [SerializeField] float springDamping = 8f;

    TempParent tempParent;  
    Rigidbody rb;             
    GravityManager gravityManager; 

    float capturedDistance;
    Coroutine gravityCoroutine;

    void Start()
    {
        rb             = GetComponent<Rigidbody>();
        tempParent     = TempParent.Instance;
        gravityManager = FindFirstObjectByType<GravityManager>();
    }

    void Update()
    {
        // quand on appuie sur F soit on ramasse l'objet soit on le lache
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isHolding)
                Drop();   
            else
                TryPickUp(); 
        }
        // avec le clic droit on lance l'objet si on le tient
        if (isHolding && Input.GetMouseButtonDown(1))
            Throw();
    }

    void FixedUpdate()
    {
        // quand on porte on applique une force de ressort pour faire suivre l'objet au tempParent devant la camera, avec un amortissement pour eviter que ça oscille trop
        if (!isHolding) return;

        // calcule disrtance / force / amortissement
        Vector3 targetPos  = tempParent.transform.position;
        Vector3 currentPos = this.transform.position;      
        Vector3 displacement = targetPos - currentPos;
        Vector3 springForce = displacement * springStrength;
        Vector3 dampingForce = -rb.linearVelocity * springDamping;

        // force vers le tempsParent
        rb.AddForce(springForce + dampingForce, ForceMode.Force);
        rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
    }

    private void TryPickUp()
    {
        // raycast depuis la camera vers la position de la souris pour vérifier si on clique sur cet objet et que c'est pas trop loin
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.transform == this.transform && tempParent != null)
            {
                if (gravityCoroutine != null)
                {
                    StopCoroutine(gravityCoroutine);
                    gravityCoroutine = null;
                }
                // on regarde la distance entre la camera et l'objet  
                capturedDistance = Vector3.Distance(Camera.main.transform.position, this.transform.position);

                // quand on porte l'objet on desactive ça gravite et les collision mais il reste en trigger, et on annule sa vitesse 
                isHolding             = true;
                rb.useGravity         = false;  
                rb.isKinematic        = false;   
                rb.detectCollisions   = true;   
                rb.linearVelocity     = Vector3.zero; 
                rb.angularVelocity    = Vector3.zero; 

                // garde la distance de l'objet et la camera 
                tempParent.SetHoldDistance(capturedDistance);
            }
        }
    }

    private void Throw()
    {
        // projette l'objet avec une force dans le direction ou l'on regarde
        isHolding = false;
        rb.linearVelocity = Vector3.zero;
        gravityCoroutine = StartCoroutine(ApplyCustomGravity());

        Vector3 throwDirection = Camera.main.transform.forward;
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }

    private void Drop()
    {
        // lache l'objet sans force 
        if (isHolding)
        {
            isHolding         = false;
            rb.linearVelocity = Vector3.zero;

            gravityCoroutine = StartCoroutine(ApplyCustomGravity());
        }
    }

    // Systeme ou l'on fait en sorte de laisser une nouvelle gravite a l'objet
    private System.Collections.IEnumerator ApplyCustomGravity()
    {
        Vector3 gravityDir = gravityManager != null ?
            gravityManager.gravityDirection.normalized : Vector3.down;

        while (!isHolding)
        {
            rb.AddForce(gravityDir * 9.81f, ForceMode.Acceleration);
            yield return null; 
        }

        gravityCoroutine = null;
    }
}
