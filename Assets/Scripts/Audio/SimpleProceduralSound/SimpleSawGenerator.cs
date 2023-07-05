using UnityEngine;

public class SimpleSawGenerator : SimpleOscillator
{
    [SerializeField, Range(0, 1)] private float amplitude = 0.5f;

    protected override float GetPhaseValue()
    {
        return (((float) _phase * 2) % 2 - 1) * amplitude;
    }
}
