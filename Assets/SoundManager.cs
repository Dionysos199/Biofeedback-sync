using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource source;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string name)
    {
        // source.Play();
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
