using UnityEngine;

public class S_SoundManager : MonoBehaviour
{
    public static S_SoundManager instance;

    [Header("Sons du joueur")]
    public AudioSource jumpAudioSource;
    public AudioSource gravityAudioSource;

    void Awake()
    {
        instance = this;
    }

    public void PlayJump()
    {
        if (jumpAudioSource != null)
            jumpAudioSource.Play();
    }

    public void PlayGravityChange()
    {
        if (gravityAudioSource != null)
            gravityAudioSource.Play();
    }
}