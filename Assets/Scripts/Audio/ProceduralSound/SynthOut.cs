using UnityEngine;

namespace ProceduralSound
{
    public class SynthOut : MonoBehaviour
    {
        [SerializeField] private SynthProvider provider;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (provider == null) return;
            provider.FillBuffer(data, channels);
        }
    }
}
