using ProceduralSound.Native;
using Unity.Burst;
using Unity.Mathematics;

namespace ProceduralSound
{
    public class TriangleGenerator : Oscillator
    {
        private void Start()
        {
            if (_getPhaseValue != null) return;
            _getPhaseValue = BurstCompiler.CompileFunctionPointer<GetPhaseValueDelegate>(GetPhaseValue).Invoke;
        }

        protected override void ProcessBuffer(ref SynthBuffer buffer)
        {
            _phase = _burstOscillator(ref buffer, _phase, _sampleRate, amplitude, frequency);
        }

        [BurstCompile]
        private static float GetPhaseValue(double phase, float amplitude)
        {
            return (float) ((2 / math.PI) * math.asin(math.sin(phase * 2 * math.PI)) * amplitude);
        }
    }
}

