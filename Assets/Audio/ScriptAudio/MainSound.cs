using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    [System.Serializable]
    public struct AudioChannel
    {
        public string mixerParameter;
        public Slider slider;
    }

    public AudioMixer mixer;
    public AudioChannel[] channels;

    void Start()
    {
        foreach (var channel in channels)
        {
            float dB;
            if (mixer.GetFloat(channel.mixerParameter, out dB))
            {
                channel.slider.value = Mathf.Pow(10f, dB / 20f);
            }

            var c = channel;
            channel.slider.onValueChanged.AddListener(
                (val) => SetVolume(c.mixerParameter, val)
            );
        }
    }

    void SetVolume(string parameter, float sliderValue)
    {
        float dB = sliderValue > 0f ? Mathf.Log10(sliderValue) * 20f : -80f;
        mixer.SetFloat(parameter, dB);
    }
}