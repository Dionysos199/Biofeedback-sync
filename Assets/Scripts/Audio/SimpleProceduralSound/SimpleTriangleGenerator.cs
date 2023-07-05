using UnityEngine;

public class SimpleTriangleGenerator : SimpleOscillator
{
    [SerializeField, Range(0, 1)] private float amplitude = 0.5f;

    protected override float GetPhaseValue()
    {
        return (2 / Mathf.PI) * Mathf.Asin(Mathf.Sin((float) _phase * 2 * Mathf.PI)) * amplitude;
    }
}
