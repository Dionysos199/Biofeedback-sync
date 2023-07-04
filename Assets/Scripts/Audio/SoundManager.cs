using System;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public Sound[] sounds;
    [HideInInspector, Range(0f, 1f)] public float masterVolume;

    private AudioSource source;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;

                s.source.loop = s.loop;
            }

            // Apply master volume
            SetMasterVolume(masterVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        Play("UnderwaterAmbience");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound '" + name + "' not found.");
            return;
        }
        s.source.Play();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = masterVolume;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target as the used type
        var soundManager = target as SoundManager;

        // Create Editor GUI
        base.OnInspectorGUI();

        // Add master volume at the end
        float masterVolume = EditorGUILayout.Slider("Master Volume", soundManager.masterVolume, 0f, 1f);
        if (masterVolume != soundManager.masterVolume)
            soundManager.SetMasterVolume(masterVolume);
    }
}
#endif