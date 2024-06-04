using Comfast.EasyDriver;
using Comfast.EasyDriver.Ui;
using FluentAssertions;
using static Comfast.EasyDriver.DriverApi;

namespace EasyDriver.Tests.Integration;

public class FindTest {
    private const string INPUT_VALUE = "some input";
    private const string OPTION_VALUE = "some option";

    public FindTest() {
        new BrowserContent().SetBody($@"
<form id='inputForm'><input value='{INPUT_VALUE}'/></form>
<form id='selectForm'><select><option value='{OPTION_VALUE}'>some select</option></select></form>
<form id='thirdForm'><input/></form>");
    }

    [Fact] public void FindCss() {
        ShouldFindCount(S("input"), 2);
        ShouldFindCount(S("html input"), 2);
        ShouldFindCount(S("form input"), 2);
        ShouldFindCount(S("form > input"), 2);
        ShouldFindCount(S("form >> input"), 1);
        ShouldFindCount(S("form").S("input"), 1);
    }

    [Fact] public void FindXpath() {
        ShouldFind(S("//input"), INPUT_VALUE);
        ShouldFind(S("//form/input"), INPUT_VALUE);
        ShouldFind(S("//html//input"), INPUT_VALUE);
        ShouldNotFind(S("//html/head/input"));
    }

    [Fact] public void NestedCss() {
        ShouldFind(S("html").S("form").S("input"), INPUT_VALUE);
    }

    [Fact] public void NestedXpath() {
        ShouldFind(S("//html").S("//form").S("//input"), INPUT_VALUE);
    }

    [Fact] public void NestedMixedXpathAndCss() {
        ShouldFind(S("html").S(".//form[1]").S("input"), INPUT_VALUE);
        ShouldFind(S("html").S(".//form[2]").S("option"), OPTION_VALUE);
        ShouldFind(S("//body").S("#inputForm").S(".//input"), INPUT_VALUE);
        ShouldFind(S("//body").S("#selectForm").S(".//option"), OPTION_VALUE);

        ShouldFind(S("html >> .//form[1] >> input"), INPUT_VALUE);
        ShouldFind(S("html >> .//form[2] >> option"), OPTION_VALUE);
        ShouldFind(S("//body >> #inputForm >> .//input"), INPUT_VALUE);
        ShouldFind(S("//body >> #selectForm >> .//option"), OPTION_VALUE);
    }

    [Fact] public void RelativeXpath() {
        ShouldFind(S("#inputForm").S(".//input"), INPUT_VALUE);
        ShouldFind(S("#selectForm").S(".//option"), OPTION_VALUE);

        ShouldNotFind(S("#inputForm").S(".//option"));
        ShouldNotFind(S("#selectForm").S(".//input"));
    }

    [Fact] public void FindAll() {
        ShouldFindCount(S("form input"), 2);
        ShouldFindCount(S("form").S("input"), 1);
        ShouldFindCount(S("form").S("article"), 0);
    }

    [Fact] public void CrossSearch() {
        // Assert.All(
        ShouldFind(S("//form//option"), OPTION_VALUE);
        ShouldNotFind(S("//form >> .//option"));
        ShouldNotFind(S("form >> option"));
        ShouldNotFind(S("form").S("select option"));

    }

    [Fact] public void Exists() {
        Assert.True(S("//body").S("input").Exists);
        Assert.False(S("//body").S("article").Exists);
    }

    [Fact] public void ParentXpath() {
        Assert.Equal("form",
            S("option").S(".. >> ..").TagName);
    }

    // void findNth() {
    //     Assert.Equal("thirdForm",
    //         S("form").nth(3).getAttribute("id"));
    // }

    [Fact] public void IsDisplayedDoesntThrowTest() {
        Assert.True(S("#selectForm >> option").IsDisplayed);
        // Assert.False(S("#selectForm >> lol >> option").IsDisplayed); // 2nd element not found
        // Assert.False(S("#selectForm >> option >> lol").IsDisplayed); // 3rd element not found
    }

    [Fact] void Count() {
        ShouldFindCount(S("form"), 3);
        ShouldFindCount(S("form input"), 2);
        ShouldFindCount(S("//form//input"), 2);
        ShouldFindCount(S("form").S("input"), 1);
        ShouldFindCount(S("form").S(".//input"), 1);
        ShouldFindCount(S("form").S("article"), 0);
    }

    [Fact] void AutoAddDotInNestedXpath() {
        //here, dot is added to second selector
        ShouldFindCount(S("//form").S("//input"), 1);
        ShouldFindCount(S("//form//input"), 2);
    }

    [Fact] void FailedFindError() {
        ShouldThrow(() => S("#selectForm >> xdxd").Find(),
            "#selectForm >> xdxd\n               ^");
    }

    private void ShouldThrow<T>(Func<T> func, string expectedMessage) {
        func.Should().Throw<Exception>()
            .Where(e => e.Message.Contains(expectedMessage));
    }

    private void ShouldFind(ILocator locator, String expectedText) {
        Assert.Equal(expectedText,
            locator.GetAttribute("value")); // format("should find '%s' in:\n'%s'", expectedText, locator)
    }

    private void ShouldFindCount(ILocator locator, int expectedCount) {
        Assert.Equal(locator.Count,
            expectedCount); //format("should find %d matches of locator:\n%s", expectedCount, locator)
    }

    private void ShouldNotFind(ILocator locator) {
        Assert.False(locator.Exists, "should not find: " + locator);
    }

}