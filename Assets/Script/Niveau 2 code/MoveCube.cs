using UnityEngine;

public class MoveCube : MonoBehaviour
{
    [SerializeField] private float speed = 50f; 
    private Vector3 randomDirection; 

    
    void Start()
    {
       
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);

        
        randomDirection = new Vector3(randomX, randomY, randomZ).normalized;
    }
    void Update()
    {
        transform.Rotate(randomDirection * speed * Time.deltaTime);
    }
}