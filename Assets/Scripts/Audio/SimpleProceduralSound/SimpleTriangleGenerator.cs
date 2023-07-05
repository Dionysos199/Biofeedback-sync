using Unity.Mathematics;

public class SimpleTriangleGenerator : SimpleOscillator
{
    protected override float GetPhaseValue()
    {
        return (float) ((2 / math.PI) * math.asin(math.sin(_phase * 2 * math.PI)));
    }
}
