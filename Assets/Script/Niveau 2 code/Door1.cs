using UnityEngine;
using System.Collections;

public class Door1 : MonoBehaviour
{
    public GameObject door;
    private bool _triggered = false;
    private void Start()
    {
        door.SetActive(true);
    }
    public void Open()
    {
        if (_triggered || door == null) return;
        _triggered = true;
        Destroy(door);
        Debug.Log("Door opened!");
    }
}