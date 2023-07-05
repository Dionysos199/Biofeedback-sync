using ProceduralSound.Native;
using Unity.Burst;
using UnityEngine;

namespace ProceduralSound
{
    public abstract class Oscillator : SynthProvider
    {
        [SerializeField, Range(0, 1)] protected float amplitude = 0.5f;
        [SerializeField, Range(16.35f, 7902.13f)] protected float frequency = 261.62f; // middle C

        protected static BurstOscillatorDelegate _burstOscillator;
        protected static GetPhaseValueDelegate _getPhaseValue;

        protected double _phase;
        protected int _sampleRate;

        private void Awake()
        {
            _sampleRate = AudioSettings.GetConfiguration().sampleRate; // 48 kHz default
            if (_burstOscillator != null) return;
            _burstOscillator = BurstCompiler.CompileFunctionPointer<BurstOscillatorDelegate>(BurstOscillator).Invoke;
        }

        protected delegate float GetPhaseValueDelegate(double phase, float amplitude);

        protected delegate double BurstOscillatorDelegate(ref SynthBuffer buffer, double phase, int sampleRate,
            float amplitude, float frequency);

        [BurstCompile]
        protected static double BurstOscillator(ref SynthBuffer buffer, double phase, int sampleRate,
            float amplitude, float frequency)
        {
            // calculate how much the phase should change after each sample
            double phaseIncrement = frequency / sampleRate;

            for (int sample = 0; sample < buffer.Length; sample += buffer.Channels)
            {
                // get value of phase on a sine wave
                float value = _getPhaseValue(phase, amplitude);

                // increment phase value for next iteration
                phase = (phase + phaseIncrement) % 1;

                // populate all channels with the values
                for (int channel = 0; channel < buffer.Channels; channel++)
                {
                    buffer[sample + channel] = value;
                }
            }

            // return the updated phase
            return phase;
        }
    }
}

