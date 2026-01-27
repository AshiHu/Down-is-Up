using UnityEngine;

public class GestionFlèche : MonoBehaviour
{
    public GameObject[] mesFleches;

    public void ActiverFleche(int index)
    {
        foreach (GameObject f in mesFleches)
        {
            f.SetActive(false);
        }

        if (index >= 0 && index < mesFleches.Length)
        {
            mesFleches[index].SetActive(true);
        }
    }

    public void CacherToutesLesFleches()
    {
        foreach (GameObject f in mesFleches)
        {
            f.SetActive(false);
        }
    }
}