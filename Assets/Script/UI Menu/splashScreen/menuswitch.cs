using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Image splashImage;
    [SerializeField] private Image blackOverlay;  // Image noire par-dessus tout
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string nextScene = "MainMenu";

    private void Start()
    {
        // S'assure que l'overlay est transparent au départ
        Color c = blackOverlay.color;
        c.a = 0f;
        blackOverlay.color = c;

        StartCoroutine(SplashRoutine());
    }

    private IEnumerator SplashRoutine()
    {
        audioSource.Play();

        // Attend 4 secondes (durée de la musique)
        yield return new WaitForSeconds(4f);

        // Fondu au noir sur 1 seconde
        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(nextScene);
    }

    private IEnumerator FadeToBlack()
    {
        float duration = 1f;
        float elapsed = 0f;
        Color c = blackOverlay.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            blackOverlay.color = c;
            yield return null;
        }

        c.a = 1f;
        blackOverlay.color = c;
    }
}