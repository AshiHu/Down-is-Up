using UnityEngine;
using UnityEngine.Audio;

public class ButtonClick : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip soundClick;
    public void PlayClick()
    {
        if (soundClick != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundClick);
        }
    }
}
