namespace Comfast.EasyDriver.Core.Events;

/// <summary> Calculates Min/Max/Total/Avg of counted times</summary>
public class TimesCounter {
    public long Count { get; private set; }
    public TimeSpan Min { get; private set; }
    public TimeSpan Max { get; private set; }
    public TimeSpan Total { get; private set; }
    public TimeSpan Average => Total / Count;

    public void CountTime(TimeSpan time) {
        Count++;
        Total += time;
        if (Min > time) Min = time;
        else if (Max < time) Max = time;
    }
}