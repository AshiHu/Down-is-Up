using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Dissolve Settings")]
    private Renderer Rend;
    private Material doorMaterial;

    [Tooltip("Durée de l'effet en secondes")]
    public float dissolveDuration = 1f;

    private bool _triggered = false;

    private void Start()
    {
        Rend = GetComponent<Renderer>();
        if (Rend != null)
        {
            doorMaterial = Rend.material;
            Debug.Log("Material : " + doorMaterial.name);
        }
        else
        {
            Debug.LogError("Il manque un MeshRenderer ou SpriteRenderer sur ce GameObject !");
        }
    }
    public void Open()
    {
        if (_triggered || doorMaterial == null) return;
        _triggered = true;
        StartCoroutine(Dissolve());
    }

    private IEnumerator Dissolve()
    {
        float elapsed = 0f;
        doorMaterial.SetFloat("_Dessolve", 0f);

        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / dissolveDuration);
            doorMaterial.SetFloat("_Dessolve", progress);
            yield return null;
        }

        doorMaterial.SetFloat("_Dessolve", 1f);
    }
}