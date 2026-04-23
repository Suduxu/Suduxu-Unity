public class JoystickData
{
    public float x;
    public float y;
    public float angleDeg;
    public float magnitude;

    public override string ToString()
    {
        return $"JoystickData{{x={x}, y={y}, angleDeg={angleDeg}, magnitude={magnitude}}}";
    }
}