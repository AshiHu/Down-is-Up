using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapFinish : MonoBehaviour
{
    [Header("Animation")]
    public float punchDuration = 0.3f;
    public float fadeDuration = 1f;
    public float flashDelay = 2f;

    [Header("Références")]
    public Image fadeImage;
    public AudioSource audioSource;
    public AudioClip goalSound;
    public string menuSceneName = "Menu";

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(GoalSequence());
    }

    IEnumerator GoalSequence()
    {
        if (goalSound != null)
            audioSource.PlayOneShot(goalSound);

        yield return new WaitForSeconds(flashDelay);

        yield return StartCoroutine(ScalePunch());

        yield return StartCoroutine(FadeToBlack());

        if (goalSound != null)
        {
            float remaining = goalSound.length - flashDelay - fadeDuration - punchDuration;
            if (remaining > 0f)
                yield return new WaitForSeconds(remaining);
        }

        Debug.Log("Sauvegardé : " + PlayerPrefs.GetInt("Niveau1finish", 0));
        PlayerPrefs.SetInt("Niveau1finish", 1); 
        PlayerPrefs.Save();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
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
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage == null)
        {
            yield break;
        }

        Color c = fadeImage.color;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
    }
}