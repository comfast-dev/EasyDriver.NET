using Comfast.EasyDriver;

namespace Comfast.Commons.Utils {
    public class WaitUtils {
        public static void WaitFor(Func<bool> action, string? description = null, int? timeoutMs = null) {
            var timeout = timeoutMs ?? Configuration.LocatorConfig.TimeoutMs;
            var end = DateTime.Now.AddMilliseconds(timeout);
            while (DateTime.Now < end) {
                try {
                    bool res = action.Invoke();
                    if (res) return;
                } catch (Exception) {
                    // ignored
                }

                Thread.Sleep(300);
            }

            int timeoutSec = timeout / 1000;
            string? descr = description ?? action.ToString();
            throw new Exception($"Wait failed after {timeoutSec}s, for: {descr}");
        }
    }
}