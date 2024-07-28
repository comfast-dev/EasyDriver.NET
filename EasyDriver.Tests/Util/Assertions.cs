using Comfast.EasyDriver.Models;
using FluentAssertions;

namespace EasyDriver.Tests.Util;

public static class Assertions {

    public static void ShouldThrow<T>(Func<T> func, string expectedMessage) {
        func.Should().Throw<Exception>()
            .Where(e => e.Message.Contains(expectedMessage));
    }

    public static void ShouldHaveValue(ILocator locator, string expectedValue) {
        locator.GetAttribute("value").Should().Match(expectedValue);
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
}