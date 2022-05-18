namespace Spam.Classes;

public class UserPressure
{
    public UserPressure(decimal pressureValue)
    {
        PressureValue = pressureValue;
    }

    public UserPressure(string content)
    {
        RecentMessages.Add(content.ToLower());
    }

    public List<string> RecentMessages { get; set; } = new();
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

    public void AddMessage(string content)
    {
        // keep five most recent messages only
        if (RecentMessages.Count > 4)
        {
            RecentMessages.RemoveAt(0);
        }

        RecentMessages.Add(content.ToLower());
    }
}