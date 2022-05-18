using DSharpPlus.Entities;

namespace Spam.Classes;

public class UserPressure
{
    private string _lastMessage;

    public UserPressure(decimal currentPressure)
    {
        CurrentPressure = currentPressure;
        _lastMessage = string.Empty;
        LastUpdate = DateTime.Now;
    }

    public List<DiscordMessage> PressureSessionMessages { get; set; } = new();

    public string LastMessage
    {
        get => _lastMessage;
        set => _lastMessage = value.ToLower();
    }

    public decimal CurrentPressure { get; private set; }
    private DateTime LastUpdate { get; set; }

    public void IncreasePressure(decimal addValue, decimal pressureDecay)
    {
        var n = DateTime.Now;
        var seconds = Convert.ToInt32((n - LastUpdate).TotalSeconds);
        var decayValue = decimal.Multiply(seconds, pressureDecay);

        if (decayValue > CurrentPressure)
        {
            // if decay exceeds pressure, start new pressure session
            PressureSessionMessages = new List<DiscordMessage>();
            CurrentPressure = addValue;
        }
        else
        {
            // if it does not exceed pressure, first decrease pressure by current decay, then add new value
            CurrentPressure = CurrentPressure - decayValue + addValue;
        }

        LastUpdate = n;
    }

    public void ResetPressure()
    {
        CurrentPressure = 0;
        LastUpdate = DateTime.Now;
        LastMessage = string.Empty;
        PressureSessionMessages = new List<DiscordMessage>();
    }
}