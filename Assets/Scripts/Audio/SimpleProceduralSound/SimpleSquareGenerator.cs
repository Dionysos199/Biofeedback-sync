using Unity.Mathematics;

public class SimpleSquareGenerator : SimpleOscillator
{
    protected override float GetPhaseValue()
    {
        return (float) (math.sign(math.sin(_phase * 2 * math.PI)));
    }
}
