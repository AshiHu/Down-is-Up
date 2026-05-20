using UnityEngine;
using System.Collections;

public class PlanetManager : MonoBehaviour
{
    [Header("Planets")]
    public GameObject Planet1;
    public GameObject Planet2;
    public GameObject Planet3;

    IEnumerator Start()
    {
        yield return null; // attend une frame

        Debug.Log("Lu : " + PlayerPrefs.GetInt("Niveau1finish", 0));
        Debug.Log("Planet1 assigné : " + (Planet1 != null));

        if (Planet1 != null)
            Planet1.SetActive(PlayerPrefs.GetInt("Niveau1finish", 0) == 1);
        if (Planet2 != null)
            Planet2.SetActive(PlayerPrefs.GetInt("Niveau2finish", 0) == 1);
        if (Planet3 != null)
            Planet3.SetActive(PlayerPrefs.GetInt("Niveau3finish", 0) == 1);
    }
}