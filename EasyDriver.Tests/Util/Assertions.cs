using Comfast.EasyDriver.Models;
using FluentAssertions;

namespace EasyDriver.Tests.Util;

public static class Assertions {
    public static void ShouldThrow(Action func, string expectedErrorMessage) {
        func.Should().Throw<Exception>()
            .Where(e => e.Message.Contains(expectedErrorMessage));
    }
    public static void ShouldThrow<T>(Func<T> func, string expectedErrorMessage) {
        func.Should().Throw<Exception>()
            .Where(e => e.Message.Contains(expectedErrorMessage));
    }

    public static void ShouldNotThrow<T>(Func<T> func) {
        func.Should().NotThrow();
    }

    public static void ShouldHaveValue(ILocator locator, string expectedValue) {
        var value = locator.GetAttribute("value");
        if (expectedValue.Length == 0) value.Should().BeEmpty();
        else value.Should().Match(expectedValue);
    }

    public static void ShouldHaveText(ILocator locator, string expectedText) {
        locator.Text.Should().Match(expectedText);
    }

    public static void ShouldFindCount(ILocator locator, int expectedCount) {
        locator.Count.Should().Be(expectedCount);
    }

    public static void ShouldNotFind(ILocator locator) {
        Assert.False(locator.Exists, "should not find: " + locator);
    }

    public static void ShouldEqual(object actual, object expected) {
        actual.Should().Be(expected);
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