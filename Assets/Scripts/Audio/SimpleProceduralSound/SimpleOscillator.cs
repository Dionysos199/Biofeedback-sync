using UnityEngine;

public abstract class SimpleOscillator : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float amplitude = 0.5f;
    [SerializeField, Range(16.35f, 7902.13f)] private float frequency = 261.62f; // middle C

    protected double _phase;
    private int _sampleRate;

    private void Awake()
    {
        _sampleRate = AudioSettings.outputSampleRate; // 48 kHz default
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        // calculate how much the phase should change after each sample
        double phaseIncrement = frequency / _sampleRate;

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            // get value of phase on a sine wave
            float value = GetPhaseValue() * amplitude;

            // increment phase value for next iteration
            _phase = (_phase + phaseIncrement) % 1;

            // populate all channels with the values
            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] = value;
            }
        }
    }

    protected abstract float GetPhaseValue();
}
