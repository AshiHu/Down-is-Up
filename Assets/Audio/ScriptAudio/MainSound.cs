using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;    

public class MainSound : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volumeSlider;

    void Start()
    {
        float value;
        mixer.GetFloat("MainMixer", out value);
        volumeSlider.value = Mathf.Pow(10, value / 20); 
    }

    public void SetVolume(float sliderValue)
    {
        float dB = Mathf.Log10(sliderValue) * 20;

        if (sliderValue <= 0) dB = -80f;

        mixer.SetFloat("MainMixer", dB);
    }
}