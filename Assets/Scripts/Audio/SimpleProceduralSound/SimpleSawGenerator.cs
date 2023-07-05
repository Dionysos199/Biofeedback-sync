using Unity.Mathematics;

public class SimpleSawGenerator : SimpleOscillator
{
    protected override float GetPhaseValue()
    {
        return (float) ((_phase * 2) % 2 - 1);
    }
}
