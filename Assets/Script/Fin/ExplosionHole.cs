using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExplosionHole : MonoBehaviour
{
    [Header("Animation")]
    public float punchDuration = 0.3f;
    public float flashDelay = 2f;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Explosion")]
    public ParticleSystem explosionParticles;
    public AudioSource explosionAudioSource;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("oui")) return;

        triggered = true;
        StartCoroutine(ScalePunch());
        
    }

    IEnumerator ScalePunch()
    {
        Vector3 baseScale = transform.localScale;
        float t = 0f;

        while (t < punchDuration * 0.5f)
        {
            t += Time.deltaTime;
            float ratio = t / (punchDuration * 0.5f);
            transform.localScale = Vector3.Lerp(baseScale, baseScale * 1.5f, ratio);
            yield return null;
        }

        t = 0f;
        while (t < punchDuration * 0.5f)
        {
            t += Time.deltaTime;
            float ratio = t / (punchDuration * 0.5f);
            transform.localScale = Vector3.Lerp(baseScale * 1.5f, Vector3.zero, ratio);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        yield return StartCoroutine(Explosion());
    }
    IEnumerator Explosion()
    { 
        if (explosionParticles != null)
        {
            explosionParticles.Play();
        }

        if (explosionAudioSource != null)
        {
            explosionAudioSource.Play();
        }
        yield return null;
    }
}