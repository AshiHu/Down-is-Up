using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Dessolve")]
    public Material doorMaterial;
    public float dessolveDuration = 0f;

    //gestion de l'animation 
    [Header("Animation")]
    public Vector3 openOffset = new Vector3(0f, 3f, 0f);
    public float duration = 1f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool _opened = false;

    // ouverture de la porte 
    public void Open()
    {
        if (_opened) return;
        _opened = true;
        StartCoroutine(Slide(transform.position + openOffset));
    }

    // animation de la porte du point start a la position voulue
    private IEnumerator Slide(Vector3 target)
    {
        doorMaterial.SetFloat("_DissolveAmount", 0f);

        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(elapsed / duration);

            // utilisation d'un lerp pour un deplacement image par image pour de la fluidite
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        doorMaterial.SetFloat("_DissolveAmount", 1f);
    }
}