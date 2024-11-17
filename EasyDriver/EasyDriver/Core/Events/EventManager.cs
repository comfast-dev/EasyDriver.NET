using System.Diagnostics;

namespace Comfast.EasyDriver.Core.Events;

public class EventManager {
    public EventStats? Stats { get; }
    private readonly ThreadLocal<Stack<string>> _actionStack = new(() => new(), true);

    public string Name { get; }
    public event EventHandler<BeforeEventArgs>? BeforeEvents;
    public event EventHandler<AfterEventArgs>? AfterEvents;
    public event EventHandler<FailedEventArgs>? FailedEvents;

    public EventManager(string name, bool collectEventStats = false) {
        Name = name;

        if (collectEventStats) {
            Stats = new(name);
            AfterEvents += (obj, args) => Stats.CountEvent(args);
        }
    }

    public T CallAction<T>(object subject, string actionName, Func<T> func, object? arguments = null) {
        var stack = _actionStack.Value!;
        BeforeEventArgs beforeArgs = new() { Name = actionName, Lvl = stack.Count, Arguments = arguments };

        stack.Push(actionName);
        try {
            if (BeforeEvents != null) BeforeEvents(subject, beforeArgs);
            var sw = Stopwatch.StartNew();

            var result = func();
            if (AfterEvents != null) AfterEvents(subject, beforeArgs.After(result, sw.Elapsed));
            stack.Pop();
            return result;
        } catch (Exception ex) {
            stack.Pop();
            if (FailedEvents != null) FailedEvents(subject, beforeArgs.Failed(ex));
            throw;
        }
    }

    public async Task<T> CallActionAsync<T>(object subject, string actionName, Func<Task<T>> func, object? arguments = null) {
        var stack = _actionStack.Value!;
        BeforeEventArgs beforeArgs = new() { Name = actionName, Lvl = stack.Count, Arguments = arguments};

        stack.Push(actionName);
        try {
            if (BeforeEvents != null) BeforeEvents(subject, beforeArgs);
            var sw = Stopwatch.StartNew();
            var result = await func();
            if (AfterEvents != null) AfterEvents(subject, beforeArgs.After(result, sw.Elapsed));
            stack.Pop();
            return result;
        } catch (Exception ex) {
            stack.Pop();
            if (FailedEvents != null) FailedEvents(subject, beforeArgs.Failed(ex));
            throw;
        }
    }
}