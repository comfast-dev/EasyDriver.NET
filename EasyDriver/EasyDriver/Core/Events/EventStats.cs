using System.Collections.Concurrent;
using System.Text;

namespace Comfast.EasyDriver.Core.Events;

/// <summary> Aggregates Event count and times for events. </summary>
/// <param name="name">for log message.</param>
public class EventStats(string name = "Event stats") {
    private readonly ConcurrentDictionary<string, TimesCounter> _data = new();
    public IDictionary<string, TimesCounter> Data => _data;

    public void CountEvent(AfterEventArgs evt) {
        var eventName = evt.Name;
        if (!_data.ContainsKey(eventName)) _data[eventName] = new();

        var eventStats = _data[eventName];
        lock (eventStats) eventStats.CountTime(evt.Time);
    }

    public override string ToString() => PrintStats();

    public string PrintStats() {
        var result = new StringBuilder()
            .AppendLine(name)
            .AppendLine(Headers);

        double summaryTotal = 0;
        long summaryCount = 0;
        foreach (var item in _data.OrderBy(x => x.Value.Total)) {
            var eventName = item.Key;
            var times = item.Value;
            summaryTotal += times.Total.TotalMilliseconds;
            summaryCount += times.Count;
            result.AppendLine(Line(eventName, times.Count, times.Average.TotalMilliseconds,
                times.Total.TotalMilliseconds));
        }

        var summaryLine = Line("SUMMARY", summaryCount, summaryTotal / summaryCount, summaryTotal);
        return result
            .AppendLine(summaryLine)
            .AppendLine("\n\n").ToString();
    }

    const int NameColumnLength = 40, CountColumnLength = 7, TimeColumnLength = 10;

    string Headers =>
        $"| {"Name",NameColumnLength} " +
        $"| {"Count",CountColumnLength} " +
        $"| {"Avg [ms]",TimeColumnLength} " +
        $"| {"Total [ms]",TimeColumnLength} |";

    string Line(string name, long count, double avgMs, double totalMs) =>
        $"| {name,NameColumnLength} " +
        $"| {count,CountColumnLength} " +
        $"| {avgMs.ToString(".0"),TimeColumnLength} " +
        $"| {totalMs.ToString(".0"),TimeColumnLength} |";
}