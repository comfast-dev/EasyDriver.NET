using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Finder;

namespace Comfast.EasyDriver.Se;

/// <summary> Provides waiting methods</summary>
public static class Waiter {
    private static int DefaultTimeoutMs => EasyDriverConfig.RuntimeConfig.TimeoutMs;
    private static int PoolingTimeMs => EasyDriverConfig.RuntimeConfig.PoolingTimeMs;

    /// <summary> Wait for any locator. Return index of first found one.</summary>
    /// <param name="locators">array of locators to check</param>
    /// <returns>index of found locator</returns>
    public static int WaitForAny(params ILocator[] locators) => WaitForAny(null, locators);

    /// <summary> Wait for any locator. Return index of first found one.</summary>
    /// <param name="locators">array of locators to check</param>
    /// <returns>index of found locator</returns>
    /// <param name="timeoutMs">max wait time, null value uses default from Configuration</param>
    public static int WaitForAny(int? timeoutMs = null, params ILocator[] locators) {
        string description = "Any element of these:\n- " + string.Join("\n- ", locators.Select(l => l.Selector));
        var foundIndex = WaitFor(description, () => {
            for (var i = 0; i < locators.Length; i++) {
                if (locators[i].Exists) {
                    return i;
                }
            }
            throw new("not found any");
        }, timeoutMs ?? DefaultTimeoutMs);

        return foundIndex;
    }

    /// <summary> Wait until action returns true or non-zero / empty</summary>
    /// <param name="actionName">for log info</param>
    /// <param name="action">should return true</param>
    /// <param name="timeoutMs">max wait time, null value uses default from Configuration</param>
    public static T WaitFor<T>(string actionName, Func<T?> action, int? timeoutMs = null) {
        var timeout = timeoutMs ?? DefaultTimeoutMs;
        var timeEnd = DateTime.Now.AddMilliseconds(timeout);
        T? result = default(T);
        Exception? lastError = null;
        while (DateTime.Now < timeEnd) {
            lastError = null;
            try {
                result = action.Invoke();
                if (IsTruthly(result)) return result!;
            } catch (Exception e) {
                lastError = e;
            }

            Thread.Sleep(PoolingTimeMs);
        }

        var msg = $"Wait failed after {timeout}ms for action: {actionName}";
        if (lastError != null) {
            throw new(msg + $"\nLast error: '{lastError.Message}'", lastError);
        }

        throw new(msg + $"\nLast result: '{result}'");
    }


    /// <summary> Wait for page redirections till page is stable for minimum time.</summary>
    /// <param name="minimumStableTimeMs">Minimum time without redirect or loading state</param>
    /// <param name="timeoutMs">max wait time, null value uses default from Configuration</param>
    public static void WaitForStablePage(int minimumStableTimeMs = 1000, int? timeoutMs = null) {
        var timeout = timeoutMs ?? DefaultTimeoutMs;
        var startedAt = DateTime.Now;
        var lastFailAt = DateTime.Now;
        int stableTimeMs = 0;

        while (stableTimeMs < minimumStableTimeMs) {
            Thread.Sleep(PoolingTimeMs);
            if (!IsPageReady()) lastFailAt = DateTime.Now;

            stableTimeMs = (int)(DateTime.Now - lastFailAt).TotalMilliseconds;
            var totalTimeMs = (int)(DateTime.Now - startedAt).TotalMilliseconds;
            if (totalTimeMs > timeout) {
                throw new($"Page is unstable, reloading every <{minimumStableTimeMs}ms. Timed out after {timeout}ms.");
            }
        }
    }

    /// <summary>
    /// The first call this method after redirection will always return false, because "document.prevReadyState" isn't set yet.
    /// This is used to detect redirection.
    /// Second call will return true if readyState is 'complete'
    /// </summary>
    private static bool IsPageReady() =>
        GetDriver().ExecuteJs<bool>(@"
if(document.prevReadyState == 'complete') return true;
document.prevReadyState = document.readyState;
return false;");

    private static bool IsTruthly(object? obj) =>
        obj switch {
            null => false,
            bool objBool => objBool,
            _ => true
        };
}