public class ButtonInput
{
    public bool mocked;
    public ButtonInputType type;
    public ButtonInputState state;

    public override string ToString()
    {
        return $"ButtonInput(mocked={mocked}, type={type}, state={state})";
    }
}
