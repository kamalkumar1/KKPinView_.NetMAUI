namespace KKPinView.Tests.Constants;

public class PinInputMethodTests
{
    [Fact]
    public void PinInputMethod_HasNumericKeypadValue()
    {
        var value = (int)KKPinView.Constants.PinInputMethod.NumericKeypad;
        Assert.Equal(0, value);
    }

    [Fact]
    public void PinInputMethod_HasSystemKeyboardValue()
    {
        var value = (int)KKPinView.Constants.PinInputMethod.SystemKeyboard;
        Assert.Equal(1, value);
    }

    [Fact]
    public void PinInputMethod_AllValuesAreDistinct()
    {
        var values = Enum.GetValues<KKPinView.Constants.PinInputMethod>();
        var distinctCount = values.Distinct().Count();
        Assert.Equal(values.Length, distinctCount);
    }
}
