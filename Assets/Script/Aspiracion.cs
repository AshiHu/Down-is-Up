using UnityEngine;

public class Aspiracion : MonoBehaviour
{
    [Header("Aspiration")]
    public float force = 5f;
    public bool isActive = true;

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        Vector3 direction = (transform.position - other.transform.position).normalized;
        cc.Move(direction * force * Time.deltaTime);
    }
}