using Comfast.EasyDriver;
using Comfast.EasyDriver.Ui;
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

    [Fact] public void findCss() {
        shouldFindCount(S("input"), 2);
        shouldFindCount(S("html input"), 2);
        shouldFindCount(S("form input"), 2);
        shouldFindCount(S("form > input"), 2);
        shouldFindCount(S("form >> input"), 1);
        shouldFindCount(S("form").S("input"), 1);
    }

    [Fact] public void findXpath() {
        shouldFind(S("//input"), INPUT_VALUE);
        shouldFind(S("//form/input"), INPUT_VALUE);
        shouldFind(S("//html//input"), INPUT_VALUE);
        shouldNotFind(S("//html/head/input"));
    }

    [Fact] public void nestedCss() {
        shouldFind(S("html").S("form").S("input"), INPUT_VALUE);
    }

    [Fact] public void nestedXpath() {
        shouldFind(S("//html").S("//form").S("//input"), INPUT_VALUE);
    }

    [Fact] public void nestedMixedXpathAndCss() {
        shouldFind(S("html").S(".//form[1]").S("input"), INPUT_VALUE);
        shouldFind(S("html").S(".//form[2]").S("option"), OPTION_VALUE);
        shouldFind(S("//body").S("#inputForm").S(".//input"), INPUT_VALUE);
        shouldFind(S("//body").S("#selectForm").S(".//option"), OPTION_VALUE);

        shouldFind(S("html >> .//form[1] >> input"), INPUT_VALUE);
        shouldFind(S("html >> .//form[2] >> option"), OPTION_VALUE);
        shouldFind(S("//body >> #inputForm >> .//input"), INPUT_VALUE);
        shouldFind(S("//body >> #selectForm >> .//option"), OPTION_VALUE);
    }

    [Fact] public void relativeXpath() {
        shouldFind(S("#inputForm").S(".//input"), INPUT_VALUE);
        shouldFind(S("#selectForm").S(".//option"), OPTION_VALUE);

        shouldNotFind(S("#inputForm").S(".//option"));
        shouldNotFind(S("#selectForm").S(".//input"));
    }

    [Fact] public void findAll() {
        shouldFindCount(S("form input"), 2);
        shouldFindCount(S("form").S("input"), 1);
        shouldFindCount(S("form").S("article"), 0);
    }

    [Fact] public void crossSearch() {
        // Assert.All(
        shouldFind(S("//form//option"), OPTION_VALUE);
        shouldNotFind(S("//form >> .//option"));
        shouldNotFind(S("form >> option"));
        shouldNotFind(S("form").S("select option"));

    }

    [Fact] public void exists() {
        Assert.True(S("//body").S("input").Exists);
        Assert.False(S("//body").S("article").Exists);
    }

    void parentXpath() {
        Assert.Equal("form",
            S("option").S(".. >> ..").TagName);
    }

    // void findNth() {
    //     Assert.Equal("thirdForm",
    //         S("form").nth(3).getAttribute("id"));
    // }

    [Fact] public void isDisplayedDoesntThrowTest() {
        Assert.True(S("#selectForm >> option").IsDisplayed);
        // Assert.False(S("#selectForm >> lol >> option").IsDisplayed); // 2nd element not found
        // Assert.False(S("#selectForm >> option >> lol").IsDisplayed); // 3rd element not found
    }

    [Fact] void count() {
        shouldFindCount(S("form"), 3);
        shouldFindCount(S("form input"), 2);
        shouldFindCount(S("//form//input"), 2);
        shouldFindCount(S("form").S("input"), 1);
        shouldFindCount(S("form").S(".//input"), 1);
        shouldFindCount(S("form").S("article"), 0);
    }

    [Fact] void autoAddDotInNestedXpath() {
        //here, dot is added to second selector
        shouldFindCount(S("//form").S("//input"), 1);
        shouldFindCount(S("//form//input"), 2);
    }

    private void shouldFind(ILocator locator, String expectedText) {
        Assert.Equal(expectedText,
            locator.GetAttribute("value")); // format("should find '%s' in:\n'%s'", expectedText, locator)
    }

    private void shouldFindCount(ILocator locator, int expectedCount) {
        Assert.Equal(locator.Count,
            expectedCount); //format("should find %d matches of locator:\n%s", expectedCount, locator)
    }

    private void shouldNotFind(ILocator locator) {
        Assert.False(locator.Exists, "should not find: " + locator);
    }

}