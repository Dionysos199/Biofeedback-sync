using UnityEngine;

public class SimpleSineGenerator : SimpleOscillator
{
    [SerializeField, Range(0, 1)] private float amplitude = 0.5f;

    protected override float GetPhaseValue()
    {
        return Mathf.Sin((float) _phase * 2 * Mathf.PI) * amplitude;
    }
}
