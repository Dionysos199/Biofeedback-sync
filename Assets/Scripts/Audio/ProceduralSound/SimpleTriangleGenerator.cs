using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTriangleGenerator : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float amplitude = 0.5f;
    [SerializeField, Range(16.35f, 7902.13f)] private float frequency = 261.62f; // middle C

    private double _phase;
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
            float value = (2 / Mathf.PI) * Mathf.Asin(Mathf.Sin((float) _phase * 2 * Mathf.PI)) * amplitude;

            // increment phase value for next iteration
            _phase = (_phase + phaseIncrement) % 1;

            // populate all channels with the values
            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] = value;
            }
        }
    }
}


