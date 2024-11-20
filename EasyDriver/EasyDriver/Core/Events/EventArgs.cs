namespace Comfast.EasyDriver.Core.Events;

public class BeforeEventArgs : EventArgs {
    public required string Name { get; init; }
    public string ThreadId { get; } = Thread.CurrentThread.ManagedThreadId.ToString();
    public object? Arguments { get; init; }
    public int Lvl { get; init; }
    public long StartTimestamp { get; init; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

    public string ArgumentsString() {
        if (Arguments == null)
            return "EMPTY";
        if (Arguments is object[] args)
            return string.Join(", ", args.Select(a => a.ToString()));
        if (Arguments is Dictionary<string, object> dict)
            return string.Join(", ", dict.Select(a => $"{a.Key}={a.Value}"));
        throw new("Not handles arguments: of type " + Arguments.GetType().Name);
    }

    internal AfterEventArgs After(object? result, TimeSpan timeElapsed) {
        return new() {
            Name = Name, Lvl = Lvl, Arguments = Arguments, StartTimestamp = StartTimestamp,
            Time = timeElapsed,
            Result = result
        };
    }

    internal FailedEventArgs Failed(Exception ex) {
        return new() {
            Name = Name, Lvl = Lvl, Arguments = Arguments, StartTimestamp = StartTimestamp,
            Cause = ex
        };
    }

    public override string ToString() => $"[{Lvl}]{Name}";
}

public class FailedEventArgs : BeforeEventArgs {
    public required Exception Cause { get; init; }
}

public class AfterEventArgs : BeforeEventArgs {
    public TimeSpan Time { get; init; }
    public object? Result { get; init; }
}