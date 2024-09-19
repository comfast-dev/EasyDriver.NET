using Comfast.EasyDriver.Models;
using FluentAssertions;

namespace EasyDriver.Tests.Util;

public static class Assertions {
    public static void ShouldThrow(Action func, string expectedErrorMessage) {
        var ex = func.Should().Throw<Exception>().Which;
        Assert.Contains(expectedErrorMessage, ex.Message);
    }

    public static void ShouldThrow<T>(Func<T> func, string expectedErrorMessage) {
        var ex = func.Should().Throw<Exception>().Which;
        Assert.Contains(expectedErrorMessage, ex.Message);
    }

    public static void ShouldThrowEquals<T>(Func<T> func, string expectedErrorMessage) {
        var ex = func.Should().Throw<Exception>().Which;
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    public static void ShouldNotThrow<T>(Func<T> func) {
        func.Should().NotThrow();
    }

    public static void ShouldHaveValue(ILocator locator, string expectedValue) {
        Assert.Equal(expectedValue, locator.GetAttribute("value"));
    }

    public static void ShouldHaveText(ILocator locator, string expectedText) {
        Assert.Equal(expectedText, locator.Text);
    }

    public static void ShouldFindCount(ILocator locator, int expectedCount) {
        Assert.Equal(expectedCount, locator.Count);
    }

    public static void ShouldNotFind(ILocator locator) {
        Assert.False(locator.Exists, "should not find: " + locator);
    }

    public static void ShouldEqual(object actual, object expected) {
        Assert.Equal(expected, actual);
    }

    public static double ShouldEndInTime(Action func, int minTimeMs, int maxTimeMs) {
        var startedAt = DateTime.Now;
        func.Should().NotThrow();

        var executionTimeMs = DateTime.Now.Subtract(startedAt).TotalMilliseconds;
        executionTimeMs.Should()
            .BeGreaterThan(minTimeMs)
            .And.BeLessThan(maxTimeMs);
        return executionTimeMs;
    }

    public static double ShouldThrowInTime(Action func, int minTimeMs, int maxTimeMs, string expectedErrorMessage) {
        var startedAt = DateTime.Now;
        func.Should().Throw<Exception>()
            .Where(e => e.Message.Contains(expectedErrorMessage));

        var executionTimeMs = DateTime.Now.Subtract(startedAt).TotalMilliseconds;
        executionTimeMs.Should()
            .BeGreaterThan(minTimeMs)
            .And.BeLessThan(maxTimeMs);
        return executionTimeMs;
    }
}