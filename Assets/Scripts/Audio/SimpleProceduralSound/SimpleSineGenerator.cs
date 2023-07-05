using Unity.Mathematics;

public class SimpleSineGenerator : SimpleOscillator
{
    protected override float GetPhaseValue()
    {
        return (float) (math.sin(_phase * 2 * math.PI));
    }
}
