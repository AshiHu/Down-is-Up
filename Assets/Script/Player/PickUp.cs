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
    Coroutine gravityCoroutine; // On garde une référence à la coroutine

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tempParent = TempParent.Instance;
        gravityManager = FindFirstObjectByType<GravityManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isHolding)
                Drop();
            else
                TryPickUp();
        }

        if (isHolding && Input.GetMouseButtonDown(1))
            Throw();
    }

    void FixedUpdate()
    {
        if (!isHolding) return;

        Vector3 targetPos = tempParent.transform.position;
        Vector3 currentPos = this.transform.position;

        Vector3 displacement = targetPos - currentPos;
        Vector3 springForce = displacement * springStrength;
        Vector3 dampingForce = -rb.linearVelocity * springDamping;

        rb.AddForce(springForce + dampingForce, ForceMode.Force);
        rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
    }

    private void TryPickUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.transform == this.transform && tempParent != null)
            {
                // On stoppe la coroutine si elle tourne encore
                if (gravityCoroutine != null)
                {
                    StopCoroutine(gravityCoroutine);
                    gravityCoroutine = null;
                }

                capturedDistance = Vector3.Distance(Camera.main.transform.position, this.transform.position);
                isHolding = true;
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                tempParent.SetHoldDistance(capturedDistance);
            }
        }
    }

    private void Throw()
    {
        isHolding = false;
        rb.linearVelocity = Vector3.zero;
        gravityCoroutine = StartCoroutine(ApplyCustomGravity());
        Vector3 throwDirection = Camera.main.transform.forward;
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }

    private void Drop()
    {
        if (isHolding)
        {
            isHolding = false;
            rb.linearVelocity = Vector3.zero;
            gravityCoroutine = StartCoroutine(ApplyCustomGravity());
        }
    }

    private System.Collections.IEnumerator ApplyCustomGravity()
    {
        // On capture la gravité AU MOMENT du lâcher, elle ne changera plus pour cet objet
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