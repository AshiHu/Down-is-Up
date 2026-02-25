using UnityEngine;

public class PickUp : MonoBehaviour
{
    bool isHolding = false;
    [SerializeField] float throwForce = 600f;
    [SerializeField] float maxDistance = 3f;
    float distance;
    TempParent tempParent;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tempParent = TempParent.Instance;
    }

    void Update()
    {
        if (isHolding)
            Hold();

        // Appuie sur F pour prendre/lâcher
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isHolding)
                Drop();
            else
                TryPickUp();
        }
    }

    private void TryPickUp()
    {
        // Vérifie que le joueur regarde bien l'objet avec un Raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.transform == this.transform && tempParent != null)
            {
                isHolding = true;
                rb.useGravity = false;
                rb.detectCollisions = true;
                this.transform.SetParent(tempParent.transform);
            }
        }
    }

    private void Hold()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (Input.GetMouseButtonDown(1))
        {
            Throw();
        }
    }

    private void Throw()
    {
        //Jet l'objet dans la direction du regard du joueur
        isHolding = false;
        this.transform.SetParent(null);
        rb.useGravity = true;

        Vector3 throwDirection = (this.transform.position - Camera.main.transform.position).normalized;
        rb.AddForce(throwDirection * throwForce);
    }

    private void Drop()
    {
        // Lâche simplement l'objet sans appliquer de force en re untilisant la touche F
        if (isHolding)
        {
            isHolding = false;
            this.transform.SetParent(null);
            rb.useGravity = true;
        }
    }
}