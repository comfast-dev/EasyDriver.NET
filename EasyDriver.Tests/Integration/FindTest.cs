using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Ui;
using FluentAssertions;
using static Comfast.EasyDriver.DriverApi;

namespace EasyDriver.Tests.Integration;

public class FindTest {
    private const string InputValue = "some input";
    private const string OptionValue = "some option";

    public FindTest() {
        new BrowserContent().SetBody($@"
<form id='inputForm'><input value='{InputValue}'/></form>
<form id='selectForm'><select><option value='{OptionValue}'>some select</option></select></form>
<form id='thirdForm'><input/></form>");
    }

    [Fact] public void FindCss() {
        ShouldFindCount(S("input"), 2);
        ShouldFindCount(S("html input"), 2);
        ShouldFindCount(S("form input"), 2);
        ShouldFindCount(S("form > input"), 2);
        ShouldFindCount(S("form >> input"), 1);
        ShouldFindCount(S("form")._S("input"), 1);
    }

    [Fact] public void FindXpath() {
        ShouldFind(S("//input"), InputValue);
        ShouldFind(S("//form/input"), InputValue);
        ShouldFind(S("//html//input"), InputValue);
        ShouldNotFind(S("//html/head/input"));
    }

    [Fact] public void NestedCss() {
        ShouldFind(S("html")._S("form")._S("input"), InputValue);
    }

    [Fact] public void NestedXpath() {
        ShouldFind(S("//html")._S("//form")._S("//input"), InputValue);
    }

    [Fact] public void NestedMixedXpathAndCss() {
        ShouldFind(S("html")._S(".//form[1]")._S("input"), InputValue);
        ShouldFind(S("html")._S(".//form[2]")._S("option"), OptionValue);
        ShouldFind(S("//body")._S("#inputForm")._S(".//input"), InputValue);
        ShouldFind(S("//body")._S("#selectForm")._S(".//option"), OptionValue);

        ShouldFind(S("html >> .//form[1] >> input"), InputValue);
        ShouldFind(S("html >> .//form[2] >> option"), OptionValue);
        ShouldFind(S("//body >> #inputForm >> .//input"), InputValue);
        ShouldFind(S("//body >> #selectForm >> .//option"), OptionValue);
    }

    [Fact] public void RelativeXpath() {
        ShouldFind(S("#inputForm")._S(".//input"), InputValue);
        ShouldFind(S("#selectForm")._S(".//option"), OptionValue);

        ShouldNotFind(S("#inputForm")._S(".//option"));
        ShouldNotFind(S("#selectForm")._S(".//input"));
    }

    [Fact] public void FindAll() {
        ShouldFindCount(S("form input"), 2);
        ShouldFindCount(S("form")._S("input"), 1);
        ShouldFindCount(S("form")._S("article"), 0);
    }

    [Fact] public void CrossSearch() {
        ShouldFind(S("//form//option"), OptionValue);
        ShouldNotFind(S("//form >> .//option"));
        ShouldNotFind(S("form >> option"));
        ShouldNotFind(S("form")._S("select option"));
    }

    [Fact] public void Exists() {
        Assert.True(S("//body")._S("input").Exists);
        Assert.False(S("//body")._S("article").Exists);
    }

    [Fact] public void ParentXpath() {
        Assert.Equal("form", S("option")._S(".. >> ..").TagName);
        Assert.Equal("form", S("option >> .. >> ..").TagName);
        Assert.Equal("form", S("//option/../..").TagName);
    }

    [Fact] public void IsDisplayedDoesntThrowTest() {
        Assert.True(S("#selectForm >> option").IsDisplayed);
        Assert.False(S("#selectForm >> lol >> option").IsDisplayed); // 2nd element not found
        Assert.False(S("#selectForm >> option >> lol").IsDisplayed); // 3rd element not found
    }

    [Fact] void Count() {
        ShouldFindCount(S("form"), 3);
        ShouldFindCount(S("form input"), 2);
        ShouldFindCount(S("//form//input"), 2);
        ShouldFindCount(S("form")._S("input"), 1);
        ShouldFindCount(S("form")._S(".//input"), 1);
        ShouldFindCount(S("form")._S("article"), 0);
    }

    [Fact] void AutoAddDotInNestedXpath() {
        //here, dot is added to second selector
        ShouldFindCount(S("//form")._S("//input"), 1);
        ShouldFindCount(S("//form >> //input"), 1);
        ShouldFindCount(S("form >> input"), 1);
        ShouldFindCount(S("//form//input"), 2);
    }

    [Fact] void FailedFindError() {
        ShouldThrow(() => S("#selectForm >> xdxd").Find(),
            "#selectForm >> xdxd\n               ^");
    }

    [Fact] public void OpenShadowDom() {
        new BrowserContent().OpenResourceFile("test.html");
        S("my-div >> h3").Text.Should().Match("Hello from shadow");
        S("my-div >> my-div >> h5").Text.Should().Match("We need go deeper");
    }

    private void ShouldThrow<T>(Func<T> func, string expectedMessage) {
        func.Should().Throw<Exception>()
            .Where(e => e.Message.Contains(expectedMessage));
    }

    private void ShouldFind(ILocator locator, String expectedText) {
        locator.GetAttribute("value").Should().Match(expectedText);
    }

    private void ShouldFindCount(ILocator locator, int expectedCount) {
        locator.Count.Should().Be(expectedCount);
    }

    private void ShouldNotFind(ILocator locator) {
        Assert.False(locator.Exists, "should not find: " + locator);
    }
}