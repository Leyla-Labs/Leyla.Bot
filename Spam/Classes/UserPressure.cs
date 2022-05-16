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

    public decimal PressureValue { get; set; }
    public List<string> RecentMessages { get; set; } = new();

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