using Comfast.EasyDriver.Models;

namespace Comfast.EasyDriver.Se;

/// <summary>
/// Provides waiting methods
/// </summary>
public class Waiter {
    private readonly int _timeoutMs;
    private readonly int _poolingMs;

    /// <summary>
    /// Create new instance
    /// </summary>
    public Waiter(int timeoutMs, int poolingMs = 50) {
        _timeoutMs = timeoutMs;
        _poolingMs = poolingMs;
    }

    /// <summary>
    /// Wait for any locator. Return index of first found one.
    /// </summary>
    /// <param name="locators">array of locators to check</param>
    /// <returns>index of found locator</returns>
    public int WaitForAny(params ILocator[] locators) =>
        WaitFor<int?>("Any element of these:\n- " + string.Join("\n- ", locators.Select(l => l.Selector)), () => {
            for (var i = 0; i < locators.Length; i++) {
                if (locators[i].Exists) return i;
            }

            return null;
        }) ?? throw new("never happen");

    /// <summary>
    /// Executes action and wait until given locator reload
    /// </summary>
    /// <param name="locator">locator that reloads</param>
    /// <param name="duringAction">Action that trigger change</param>
    public void WaitForReload(ILocator locator, Action? duringAction = null) {
        var beforeDomId = locator.DomId;
        if(duringAction != null) duringAction.Invoke();
        WaitFor("Reload element: " + locator.Selector, () => locator.DomId != beforeDomId);
    }

    /// <summary>
    /// Wait until action returns true or non-zero / empty
    /// </summary>
    /// <param name="actionName">for log info</param>
    /// <param name="action">should return true</param>
    public T WaitFor<T>(string actionName, Func<T?> action) {
        var timeout = DateTime.Now.AddMilliseconds(_timeoutMs);
        T? result = default(T);
        Exception? lastError = null;
        while (DateTime.Now < timeout) {
            lastError = null;
            try {
                result = action.Invoke();
                if (IsTruthly(result)) return result!;
            } catch (Exception e) {
                lastError = e;
            }

            Thread.Sleep(_poolingMs);
        }

        var msg = $"Wait failed after {_timeoutMs}ms for action: {actionName}";
        if (lastError != null) {
            throw new(msg + $"\nLast error: '{lastError.Message}'", lastError);
        }

        throw new(msg + $"\nLast result: '{result}'");
    }

    /// <summary>
    /// Wait for page redirections till page is stable for minimum time.
    /// </summary>
    /// <param name="minimumStableTimeMs">Minimum time without redirect or loading state</param>
    public void WaitForStablePage(int minimumStableTimeMs = 1000) {
        var startedAt = DateTime.Now;
        var lastFailAt = DateTime.Now;
        int stableTimeMs = 0;

        while (stableTimeMs < minimumStableTimeMs) {
            Thread.Sleep(_poolingMs);
            if (!IsPageReady()) lastFailAt = DateTime.Now;

            stableTimeMs = (int)(DateTime.Now - lastFailAt).TotalMilliseconds;
            var totalTimeMs = (int)(DateTime.Now - startedAt).TotalMilliseconds;
            if (totalTimeMs > _timeoutMs) {
                throw new($"Page is unstable, loading every <{minimumStableTimeMs}ms. Timed out after {_timeoutMs}ms.");
            }
        }
    }

    /// <summary>
    /// The first call this method after redirection will always return false, because "document.prevReadyState" isn't set yet.
    /// This is used to detect redirection.
    /// Second call will return true if readyState is 'complete'
    /// </summary>
    private static bool IsPageReady() =>
        DriverApi.ExecuteJs<bool>(@"
if(document.prevReadyState == 'complete') return true;
document.prevReadyState = document.readyState;
return false;");

    private bool IsTruthly(object? obj) =>
        obj switch {
            null => false,
            bool objBool => objBool,
            _ => true
        };
}