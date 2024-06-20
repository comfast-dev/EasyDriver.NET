using Comfast.EasyDriver;

namespace Comfast.Commons.Utils;

public class WaitUtils {
    public static void WaitFor(Func<bool> action, string? description = null, int? timeoutMs = null) {
        var timeout = timeoutMs ?? Configuration.LocatorConfig.TimeoutMs;
        var end = DateTime.Now.AddMilliseconds(timeout);

        Exception? lastError = null;
        while (DateTime.Now < end) {
            try {
                bool res = action.Invoke();
                if (res) return;
            } catch (Exception e) {
                lastError = e;
            }

            Thread.Sleep(300);
        }

        int timeoutSec = timeout / 1000;
        string? descr = description ?? action.ToString();
        string? error = lastError == null ? "" : "Last error:\n " + lastError.Message;
        throw new Exception($"Wait failed after {timeoutSec}s, for: {descr}. {error}");
    }
}