namespace Spam.Classes;

public class UserPressure
{
    private string _lastMessage;

    public UserPressure(decimal pressureValue)
    {
        PressureValue = pressureValue;
        _lastMessage = string.Empty;
    }

    public string LastMessage
    {
        get => _lastMessage;
        set => _lastMessage = value.ToLower();
    }

    public decimal PressureValue { get; private set; }
    private DateTime LastUpdate { get; set; }

    public void IncreasePressure(decimal addValue, decimal pressureDecay)
    {
        var n = DateTime.Now;
        var seconds = Convert.ToDecimal((n - LastUpdate).TotalSeconds);
        var decayValue = decimal.Multiply(seconds, pressureDecay);
        decayValue = decayValue < PressureValue ? decayValue : PressureValue; // do not decrease pressure below 0
        PressureValue = PressureValue - decayValue + addValue;
        LastUpdate = n;
    }

    public void ResetPressure()
    {
        PressureValue = 0;
        LastUpdate = DateTime.Now;
        LastMessage = string.Empty;
    }
}